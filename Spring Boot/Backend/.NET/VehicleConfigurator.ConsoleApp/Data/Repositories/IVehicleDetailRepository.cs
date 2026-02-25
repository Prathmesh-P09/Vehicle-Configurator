using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Data.Repositories
{
    public interface IVehicleDetailRepository
    {
        Task<List<VehicleDetail>> FindDefaultComponentsAsync(int modelId);
        Task<List<string>> FindConfigurableVehicleDetailsAsync(int modelId, string compType);
        Task<List<VehicleDetail>> FindConfigurableComponentsAsync(int modelId, string compType);
        Task<List<VehicleDetail>> FindUniqueDefaultComponentsAsync(int modelId);
    }
}
