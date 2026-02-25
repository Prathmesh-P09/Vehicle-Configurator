using Microsoft.EntityFrameworkCore;
using project_vc_.Data;
using project_vc_.DTOs;
using project_vc_.Models;
using System.Linq;

namespace project_vc_.Services;

public interface IVehicleService
{
    // WelcomeService methods
    Task<List<SegmentDTO>> GetAllSegmentsAsync();
    Task<List<ManufacturerDTO>> GetManufacturersBySegmentAsync(int segId);
    Task<List<ModelDTO>> GetModelsAsync(int segId, int mfgId);

    // DefaultConfigService methods
    Task<DefaultConfigResponseDTO> GetDefaultConfigurationAsync(int modelId, int quantity);

    // VehicleManager methods
    Task<List<ComponentDropdownDTO>> GetConfigurableComponentsAsync(int modelId, string compType);

    // VehicalDetails methods
    Task<List<string>> GetConfigurableComponentNamesAsync(int modelId, string compType);

    // AlternateComponentManager methods
    Task SaveAlternateComponentsAsync(AlternateComponentSaveDTO dto);
}

public class VehicleService : IVehicleService
{
    private readonly ApplicationDbContext _context;

    public VehicleService(ApplicationDbContext context)
    {
        _context = context;
    }

    // WelcomeService
    public async Task<List<SegmentDTO>> GetAllSegmentsAsync()
    {
        return await _context.Segments
            .Select(s => new SegmentDTO { Id = s.Id, Name = s.SegName })
            .ToListAsync();
    }

    public async Task<List<ManufacturerDTO>> GetManufacturersBySegmentAsync(int segId)
    {
        return await _context.SgMfgMasters
            .Where(x => x.SegId == segId)
            .Include(x => x.Mfg)
            .Select(x => x.Mfg)
            .Distinct()
            .Select(m => new ManufacturerDTO { Id = m!.Id, Name = m.MfgName })
            .ToListAsync();
    }

    public async Task<List<ModelDTO>> GetModelsAsync(int segId, int mfgId)
    {
        return await _context.VehicleModels
            .Where(m => m.SegId == segId && m.MfgId == mfgId)
            .Select(m => new ModelDTO
            {
                Id = m.Id,
                Name = m.ModelName,
                Price = m.Price,
                MinQty = m.MinQty,
                ImagePath = m.ImgPath
            })
            .ToListAsync();
    }

    // DefaultConfigService
    public async Task<DefaultConfigResponseDTO> GetDefaultConfigurationAsync(int modelId, int quantity)
    {
        var model = await _context.VehicleModels
            .Include(m => m.Seg)
            .Include(m => m.Mfg)
            .FirstOrDefaultAsync(m => m.Id == modelId)
            ?? throw new Exception("Invalid model");

        // Logic from findUniqueDefaultComponents: Min ID per group
        var defaultDetails = await _context.VehicleDetails
            .Where(vd => vd.ModelId == modelId && vd.IsConfig == "Y")
            .Include(vd => vd.Comp)
            .ToListAsync();

        // Client-side grouping for complex SQL "IN (SELECT MIN...)" translation optimization or simple LINQ
        // Java used subquery. Here we can group in memory if dataset is small, or use LINQ GroupBy.
        var uniqueComponents = defaultDetails
            .Where(vd => vd.Comp != null)
            .GroupBy(vd => vd.Comp!.CompName)
            .Select(g => g.OrderBy(vd => vd.ConfigId).First())
            .Select(vd => new DefaultConfigurationDTO(vd.ConfigId, vd.Comp!.Type, vd.Comp.CompName, vd.CompType, vd.Comp.CompId))
            .ToList();

        double unitPrice = model.Price;
        double totalPrice = unitPrice * quantity;

        var response = new DefaultConfigResponseDTO(
            model.Id,
            model.ModelName,
            model.Seg?.SegName,
            model.Mfg?.MfgName,
            unitPrice,
            model.MinQty,
            totalPrice,
            model.ImgPath
        );
        response.DefaultComponents = uniqueComponents;

        return response;
    }

    // VehicleManager
    public async Task<List<ComponentDropdownDTO>> GetConfigurableComponentsAsync(int modelId, string compType)
    {
        // 1. Fetch EVERYTHING for this model and type (Standard + Alternates)
        // We need all details to correctly group them and find the defaults
        var allDetails = await _context.VehicleDetails
            .Include(vd => vd.Comp)
            .Where(vd => vd.ModelId == modelId && vd.CompType == compType)
            .ToListAsync();

        // 2. Identify the defaults in memory (based on IsConfig == 'Y')
        // We group by CompName to ensure we have one stable default per feature group
        var defaultDetails = allDetails
            .Where(vd => vd.IsConfig == "Y" && vd.Comp != null)
            .GroupBy(vd => vd.Comp!.CompName)
            .Select(g => g.OrderBy(vd => vd.ConfigId).First())
            .ToList();

        // 3. Group by Component Name for the UI
        // This merges standard components with their alternates in a single dropdown group
        var grouped = allDetails
            .Where(vd => vd.Comp != null)
            .GroupBy(vd => vd.Comp!.CompName)
            .Select(g => {
                var standardForGroup = defaultDetails.FirstOrDefault(d => d.Comp!.CompName == g.Key);
                return new ComponentDropdownDTO
                {
                    BaseCompId = standardForGroup?.Comp?.CompId ?? 0,
                    ComponentName = g.Key ?? "Unknown",
                    Options = g.Select(vd => new OptionDTO
                    {
                        CompId = vd.Comp!.CompId,
                        SubType = vd.Comp.Type,
                        Price = vd.Comp.Price
                    }).ToList()
                };
            })
            .ToList();

        return grouped;
    }

    public async Task<List<string>> GetConfigurableComponentNamesAsync(int modelId, string compType)
    {
        return await _context.VehicleDetails
            .Where(v => v.ModelId == modelId && v.IsConfig == "Y" && v.CompType == compType)
            .Include(v => v.Comp)
            .Select(v => v.Comp != null ? v.Comp.CompName : "Unknown")
            .ToListAsync();
    }

    public async Task<List<AlternateComponentMaster>> SaveAlternateComponentsAsync(AlternateComponentSaveDTO dto)
    {
        if (dto.ModelId == null) throw new Exception("Model ID missing");

        var model = await _context.VehicleModels.FindAsync(dto.ModelId)
                    ?? throw new Exception("Model not found");

        var results = new List<AlternateComponentMaster>();
        if (dto.Components != null)
        {
            foreach (var item in dto.Components)
            {
                if (item.CompId == null || item.AltCompId == null) continue;

                var original = await _context.Components.FindAsync(item.CompId)
                               ?? throw new Exception("Original component not found");

                var alternate = await _context.Components.FindAsync(item.AltCompId)
                                ?? throw new Exception("Alternate component not found");

                if (original.CompName != alternate.CompName)
                {
                    throw new Exception("Invalid component replacement");
                }

                double deltaPrice = alternate.Price - original.Price;

                // Find existing
                var acm = await _context.AlternateComponentMasters
                    .FirstOrDefaultAsync(a => a.ModelId == dto.ModelId && a.CompId == item.CompId);

                if (acm == null)
                {
                    acm = new AlternateComponentMaster
                    {
                        ModelId = dto.ModelId,
                        CompId = item.CompId
                    };
                    _context.AlternateComponentMasters.Add(acm);
                }

                acm.AltCompId = item.AltCompId;
                acm.DeltaPrice = deltaPrice;
                results.Add(acm);
            }
            await _context.SaveChangesAsync();
        }
        return results;
    }

    Task IVehicleService.SaveAlternateComponentsAsync(AlternateComponentSaveDTO dto)
    {
        return SaveAlternateComponentsAsync(dto);
    }
}
