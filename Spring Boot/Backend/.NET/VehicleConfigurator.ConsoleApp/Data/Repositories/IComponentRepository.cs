using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Data.Repositories
{
    public interface IComponentRepository
    {
        Task<Component?> FindByIdAsync(int id);
    }
}
