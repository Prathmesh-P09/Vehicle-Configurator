using Microsoft.Extensions.Caching.Memory;
using VehicleConfigurator.ConsoleApp.Data;
using VehicleConfigurator.ConsoleApp.Data.Repositories;
using VehicleConfigurator.ConsoleApp.DTOs;
using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Services
{
    public class WelcomeService : IWelcomeService
    {
        private readonly AppDbContext _context; // To access generic DbSet if needed, or use specific repos
        // Using Repositories as defined
        private readonly IManufacturerRepository _mfgRepo;
        private readonly ISgMfgMasterRepository _sgMfgRepo;
        private readonly IModelRepository _modelRepo;
        private readonly IMemoryCache _cache;

        public WelcomeService(AppDbContext context, 
                              IManufacturerRepository mfgRepo, 
                              ISgMfgMasterRepository sgMfgRepo,
                              IModelRepository modelRepo,
                              IMemoryCache cache)
        {
            _context = context;
            _mfgRepo = mfgRepo;
            _sgMfgRepo = sgMfgRepo;
            _modelRepo = modelRepo;
            _cache = cache;
        }

        public async Task<List<SegmentDto>> GetAllSegmentsAsync()
        {
            return await _cache.GetOrCreateAsync("segments", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                var segs = _context.Segments.ToList();
                return segs.Select(s => new SegmentDto { Id = s.Id, SegName = s.SegName }).ToList();
            });
        }

        public async Task<List<ManufacturerDto>> GetManufacturersAsync(int segId)
        {
            return await _cache.GetOrCreateAsync($"manufacturers_{segId}", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                var masters = await _sgMfgRepo.FindBySegIdWithManufacturerAsync(segId);
                var mfgs = masters.Select(m => m.Mfg).DistinctBy(m => m.Id).ToList();
                return mfgs.Select(m => new ManufacturerDto { Id = m.Id, MfgName = m.MfgName }).ToList();
            });
        }

        public async Task<List<ModelDto>> GetModelsAsync(int segId, int mfgId)
        {
            return await _cache.GetOrCreateAsync($"models_{segId}_{mfgId}", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                var models = await _modelRepo.FindByMfgIdAndSegIdAsync(mfgId, segId);
                 return models.Select(m => new ModelDto 
                 { 
                     Id = m.Id, 
                     ModelName = m.ModelName,
                     Price = m.Price,
                     MinQty = m.MinQty,
                     ImagePath = m.ImgPath ?? string.Empty
                 }).ToList();
            });
        }

        public async Task<Model?> GetModelByIdAsync(int modelId)
        {
             return await _modelRepo.FindByIdAsync(modelId);
        }
    }
}
