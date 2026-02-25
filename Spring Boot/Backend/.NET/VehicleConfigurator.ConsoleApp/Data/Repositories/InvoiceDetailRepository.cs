using Microsoft.EntityFrameworkCore;
using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Data.Repositories
{
    public class InvoiceDetailRepository : IInvoiceDetailRepository
    {
        private readonly AppDbContext _context;

        public InvoiceDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(InvoiceDetail detail)
        {
            _context.InvoiceDetails.Add(detail);
            await _context.SaveChangesAsync();
        }

        public async Task<List<InvoiceDetail>> FindAllByInvoiceIdAsync(int invoiceId)
        {
            return await _context.InvoiceDetails
                .Include(d => d.Comp)
                .Where(d => d.InvId == invoiceId)
                .ToListAsync();
        }
        
        public async Task<List<InvoiceDetail>> FindAllAsync()
        {
            return await _context.InvoiceDetails.Include(d => d.Inv).ToListAsync();
        }
    }
}
