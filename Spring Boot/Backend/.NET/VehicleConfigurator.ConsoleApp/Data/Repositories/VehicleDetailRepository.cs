using Microsoft.EntityFrameworkCore;
using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Data.Repositories
{
    public class VehicleDetailRepository : IVehicleDetailRepository
    {
        private readonly AppDbContext _context;

        public VehicleDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<VehicleDetail>> FindDefaultComponentsAsync(int modelId)
        {
            return await _context.VehicleDetails
                .Include(vd => vd.Comp)
                .Where(vd => vd.ModelId == modelId && vd.IsConfig == "Y")
                .ToListAsync();
        }

        public async Task<List<string>> FindConfigurableVehicleDetailsAsync(int modelId, string compType)
        {
            return await _context.VehicleDetails
                .Where(vd => vd.ModelId == modelId && vd.IsConfig == "Y" && vd.CompType == compType)
                .Select(vd => vd.Comp.CompName)
                .ToListAsync();
        }

        public async Task<List<VehicleDetail>> FindConfigurableComponentsAsync(int modelId, string compType)
        {
            return await _context.VehicleDetails
                .Include(vd => vd.Comp)
                .Where(vd => vd.ModelId == modelId && vd.IsConfig == "Y" && vd.CompType == compType)
                .ToListAsync();
        }

        public async Task<List<VehicleDetail>> FindUniqueDefaultComponentsAsync(int modelId)
        {
            // Subquery logic: Group by CompName, take Min(Id)
            // SQL: WHERE vd.id IN (SELECT MIN(id) ... GROUP BY compName)
            
            var subquery = _context.VehicleDetails
                .Where(vd => vd.ModelId == modelId && vd.IsConfig == "Y")
                .GroupBy(vd => vd.Comp.CompName)
                .Select(g => g.Min(vd => vd.ConfigId));

            return await _context.VehicleDetails
                .Include(vd => vd.Comp)
                .Where(vd => subquery.Contains(vd.ConfigId) && vd.IsConfig == "Y")
                .OrderBy(vd => vd.Comp.CompName)
                .ToListAsync();
        }
    }
}
