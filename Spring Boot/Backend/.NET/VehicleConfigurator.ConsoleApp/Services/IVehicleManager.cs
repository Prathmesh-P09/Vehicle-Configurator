using VehicleConfigurator.ConsoleApp.DTOs;
using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Services
{
    public interface IVehicleManager
    {
        Task<List<string>> GetConfigurableVehicleDetailsAsync(int modelId, string compType);
        Task<List<ComponentConfigDto>> GetConfigurableComponentsAsync(int modelId, string compType);
    }
}
