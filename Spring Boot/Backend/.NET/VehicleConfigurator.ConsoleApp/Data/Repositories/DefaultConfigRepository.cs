using Microsoft.EntityFrameworkCore;
using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Data.Repositories
{
    public class DefaultConfigRepository : IDefaultConfigRepository
    {
        private readonly AppDbContext _context;

        public DefaultConfigRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<VehicleDefaultConfig>> FindByModelIdAsync(int modelId)
        {
            return await _context.VehicleDefaultConfigs
                .Include(v => v.Comp)
                .Where(v => v.ModelId == modelId)
                .ToListAsync();
        }
    }
}
