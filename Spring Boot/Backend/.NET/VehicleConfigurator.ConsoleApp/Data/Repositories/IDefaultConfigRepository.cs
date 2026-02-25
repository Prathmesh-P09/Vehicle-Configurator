using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Data.Repositories
{
    public interface IDefaultConfigRepository
    {
        Task<List<VehicleDefaultConfig>> FindByModelIdAsync(int modelId);
    }
}
