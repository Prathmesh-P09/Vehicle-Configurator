using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Data.Repositories
{
    public interface ISgMfgMasterRepository
    {
        Task<List<SgMfgMaster>> FindBySegIdWithManufacturerAsync(int segId);
    }
}
