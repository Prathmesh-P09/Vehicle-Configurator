using Microsoft.EntityFrameworkCore;
using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Data.Repositories
{
    public class InvoiceHeaderRepository : IInvoiceHeaderRepository
    {
        private readonly AppDbContext _context;

        public InvoiceHeaderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceHeader> SaveAsync(InvoiceHeader invoice)
        {
            if (invoice.Id == 0)
                _context.InvoiceHeaders.Add(invoice);
            else
                _context.InvoiceHeaders.Update(invoice);
            
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<InvoiceHeader?> FindByIdAsync(int id)
        {
            // Might need eager loading if we display details later, but for now basic find
            return await _context.InvoiceHeaders.FindAsync(id);
        }
    }
}
