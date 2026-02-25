using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Data.Repositories
{
    public interface IInvoiceDetailRepository
    {
        Task SaveAsync(InvoiceDetail detail);
        Task<List<InvoiceDetail>> FindAllByInvoiceIdAsync(int invoiceId);
        Task<List<InvoiceDetail>> FindAllAsync(); 
    }
}
