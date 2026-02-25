using Microsoft.EntityFrameworkCore;
using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Data.Repositories
{
    public class SgMfgMasterRepository : ISgMfgMasterRepository
    {
        private readonly AppDbContext _context;

        public SgMfgMasterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SgMfgMaster>> FindBySegIdWithManufacturerAsync(int segId)
        {
            return await _context.SgMfgMasters
                .Include(sm => sm.Mfg)
                .Where(sm => sm.SegId == segId)
                .ToListAsync();
        }
    }
}
