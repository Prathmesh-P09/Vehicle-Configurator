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
            .GroupBy(vd => vd.Comp!.CompName)
            .Select(g => g.OrderBy(vd => vd.ConfigId).First())
            .Select(vd => new ComponentDTO(vd.Comp!.CompId, vd.Comp.Type)) // Note: Java mapped Comp.Type as Name/Desc? "v.getComp().getType()" logic
            .ToList();

        // Wait, Java Code: new ComponentDTO(v.comp.compId, v.comp.type)
        // Java ComponentDTO(Integer id, String name). So it passes Type as Name?
        // Let's check logic. "v.getComp().getType() // default variant".
        // Yes.

        double unitPrice = model.Price;
        double totalPrice = unitPrice * quantity;

        return new DefaultConfigResponseDTO(
            model.Id,
            model.ModelName,
            model.Seg?.SegName,
            model.Mfg?.MfgName,
            unitPrice,
            model.MinQty,
            totalPrice,
            model.ImgPath,
            uniqueComponents
        );
    }

    // VehicleManager
    public async Task<List<ComponentDropdownDTO>> GetConfigurableComponentsAsync(int modelId, string compType)
    {
        var details = await _context.VehicleDetails
            .Include(vd => vd.Comp)
            .Where(vd => vd.ModelId == modelId && vd.CompType == compType && vd.IsConfig == "Y")
            .ToListAsync();

        // Group by CompName (e.g. "Color") logic from Java
        // Java: groupedMap.computeIfAbsent(baseCompId)...
        // But logic relies on `baseCompId = vd.getComp().getCompId()`.
        // Java comments: "BASE COMPONENT".
        // Wait, Java code: `baseCompId = vd.getComp().getCompId()`
        // `Component optionComp = vd.getComp()`
        // It seems `baseCompId` comes from the component itself.
        // It groups by the component ID of the row.
        // But adds `optionComp` as an Option.
        // Logic seems to imply `vd` *is* the option?
        // Ah, `VehicleDetail` links `Model` and `Component`.
        // If multiple `VehicleDetail` rows exist for same model, they are options.
        // But how are they grouped?
        // Java code groups by `vd.getComp().getCompId()`.
        // So each `VehicleDetail` row is a group? That implies 1 option per group if ID is unique?
        // Unless `vd.getComp()` returns a "Header" component?
        // The Java code says `Integer baseCompId = vd.getComp().getCompId()`.
        // And `Component optionComp = vd.getComp()`.
        // If `vd` points to specific component (e.g. "Red Color"), then grouping by ID means grouping by "Red Color". 
        // This results in 1 item per group.
        // UNLESS `VehicleDetail` logic is different or I misread Java.
        // Let's re-read Java `VehicleManagerImpl.java` Step 212.
        /*
            for (VehicleDetail vd : details) {
                Integer baseCompId = vd.getComp().getCompId();
                String componentName = vd.getComp().getCompName();
                Component optionComp = vd.getComp();
                OptionDto option = new OptionDto(...);
                groupedMap.computeIfAbsent(baseCompId, ...).getOptions().add(option);
            }
        */
        // If `vd` iterates distinct rows, and we group by `vd.Comp.Id`, then yes, each group has 1 option.
        // Maybe the UI expects this format even if single option?
        // OR `compName` is the grouping key?
        // The map key is `baseCompId`.
        // If `VehicleDetails` contains multiple rows with SAME `CompId`? Impossible if `CompId` is PK of Component.
        // Unless `VehicleDetail` has Many-to-One to Component.
        // So `VehicleDetail` (Model X) -> Component A.
        // `VehicleDetail` (Model X) -> Component B.
        // If A and B are different, they are different groups.
        // This logic seems to return 1 option per dropdown?
        // Wait, `getConfigurableComponents`. Maybe `compType` (e.g. "I") returns multiple things like "Interior Color", "Seat Type".
        // If "Interior Color" has options "Red", "Blue", do they share a parent?
        // In this schema, `Component` has `CompName` ("Seat Belt", "Air Bag").
        // If I have "Red Seat Belt" and "Blue Seat Belt", are they 2 Components? Yes.
        // If they are 2 components, they have different IDs.
        // So they form 2 dropdowns? That seems wrong.
        // Unless `CompName` is "Seat Belt" for both?
        // If `CompName` is shared ("Seat Belt"), but IDs differ?
        // Java groups by `baseCompId`. So by ID.
        // If IDs differ, they are separate groups.
        // This implies the standard behavior is: Each component listed in `vehicle_detail` is a separate configurable item.
        // BUT where are the OPTIONS?
        // `OptionDto` is created from `vd.getComp()`.
        // `groupedMap...getOptions().add(option)`.
        // Only 1 option added per `vd`.
        // IS IT POSSIBLE `AlternateComponentMaster` is involved?
        // Java `VehicleManagerImpl` imports `ComponentDropdownDto`, `OptionDto`.
        // It does NOT use `AlternateComponentMaster` or `alternateComponentRepo` in `getConfigurableComponents`.
        // It ONLY uses `vehicleRepo.findConfigurableComponents`.
        // So this method returns simple 1-item lists?
        // Or maybe I am missing something about `CompId`.
        // Maybe `comp_id` in `VehicleDetail` refers to a "Category Component"?
        // `Component.java`: `compName` (Seat Belt).
        // If `VehicleDetail` points to "Seat Belt" component.
        // Where are the choices?
        // Maybe `getConfigurableComponents` just lists what IS configurable?
        // And the UI fetches options separately?
        // But it returns `List<ComponentDropdownDto>`. `ComponentDropdownDto` has `List<OptionDto>`.
        // If it populates `Options`, it implies multiple.
        // But loop adds 1 option.
        // Conclusion: This Java code might return lists of size 1.
        // I will replicate logic EXACTLY. (Group by CompId).
        
        var grouped = details.GroupBy(vd => vd.Comp!.CompId)
            .Select(g => new ComponentDropdownDTO
            {
                BaseCompId = g.Key,
                ComponentName = g.First().Comp!.CompName,
                Options = g.Select(vd => new OptionDTO
                {
                    CompId = vd.Comp!.CompId,
                    SubType = vd.Comp.Type,
                    Price = vd.Comp.Price
                }).ToList()
            })
            .ToList();
            
        return grouped;
    }

    public async Task<List<string>> GetConfigurableComponentNamesAsync(int modelId, string compType)
    {
        return await _context.VehicleDetails
            .Where(v => v.ModelId == modelId && v.IsConfig == "Y" && v.CompType == compType)
            .Include(v => v.Comp)
            .Select(v => v.Comp!.CompName!)
            .ToListAsync();
    }

    public async Task SaveAlternateComponentsAsync(AlternateComponentSaveDTO dto)
    {
        if (dto.ModelId == null) throw new Exception("Model ID missing");
        
        var model = await _context.VehicleModels.FindAsync(dto.ModelId)
                    ?? throw new Exception("Model not found");

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
            }
            await _context.SaveChangesAsync();
        }
    }
}
