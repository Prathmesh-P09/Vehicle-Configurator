using VehicleConfigurator.ConsoleApp.DTOs;
using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Services
{
    public interface IWelcomeService
    {
        Task<List<SegmentDto>> GetAllSegmentsAsync();
        Task<List<ManufacturerDto>> GetManufacturersAsync(int segId);
        Task<List<ModelDto>> GetModelsAsync(int segId, int mfgId); 
        Task<Model?> GetModelByIdAsync(int modelId);
    }
}
