using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Data.Repositories
{
    public interface IAlternateComponentRepository
    {
        Task<AlternateComponentMaster?> FindByModelAndCompAsync(int modelId, int compId);
        Task<double> SumDeltaPriceByModelIdAsync(int modelId);
        Task<List<AlternateComponentMaster>> FindByModelIdAsync(int modelId);
        Task SaveAsync(AlternateComponentMaster entity);
    }
}
