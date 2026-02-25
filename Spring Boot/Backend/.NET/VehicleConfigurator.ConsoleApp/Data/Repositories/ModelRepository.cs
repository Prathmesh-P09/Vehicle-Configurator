using Microsoft.EntityFrameworkCore;
using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Data.Repositories
{
    public class ModelRepository : IModelRepository
    {
        private readonly AppDbContext _context;

        public ModelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Model>> FindByMfgIdAndSegIdAsync(int mfgId, int segId)
        {
            return await _context.Models
                .Where(m => m.MfgId == mfgId && m.SegId == segId)
                .ToListAsync();
        }

        public async Task<Model?> FindByIdWithSegAndMfgAsync(int modelId)
        {
            return await _context.Models
                .Include(m => m.Seg)
                .Include(m => m.Mfg)
                .FirstOrDefaultAsync(m => m.Id == modelId);
        }

        public async Task<Model?> FindByIdAsync(int id)
        {
            return await _context.Models.FindAsync(id);
        }
    }
}
