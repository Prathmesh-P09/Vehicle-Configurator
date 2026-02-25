using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Data.Repositories
{
    public interface IInvoiceHeaderRepository
    {
        Task<InvoiceHeader> SaveAsync(InvoiceHeader invoice);
        Task<InvoiceHeader?> FindByIdAsync(int id);
    }
}
