using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Data.Repositories
{
    public interface IModelRepository
    {
        Task<List<Model>> FindByMfgIdAndSegIdAsync(int mfgId, int segId);
        Task<Model?> FindByIdWithSegAndMfgAsync(int modelId);
        Task<Model?> FindByIdAsync(int id);
    }
}
