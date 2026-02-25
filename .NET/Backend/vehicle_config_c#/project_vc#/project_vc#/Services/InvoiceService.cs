using Microsoft.EntityFrameworkCore;
using project_vc_.Data;
using project_vc_.DTOs;
using project_vc_.Models;
using project_vc_.Utils;

namespace project_vc_.Services;

public interface IInvoiceService
{
    Task GenerateInvoiceAsync(InvoiceRequestDTO request, string username);
}

public class InvoiceService : IInvoiceService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public InvoiceService(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task GenerateInvoiceAsync(InvoiceRequestDTO dto, string username)
    {
        if (dto.ModelId == null) throw new Exception("Model ID missing");
        if ((dto.Qty ?? 0) <= 0) throw new Exception("Invalid quantity");

        // User
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username || u.Email == username)
                   ?? throw new Exception("User not found");

        // Model
        var model = await _context.VehicleModels.FindAsync(dto.ModelId)
                    ?? throw new Exception("Model not found");

        // Price Calc
        double baseAmt = model.Price * dto.Qty!.Value;
        double addOnTotal = 0.0;
        var selectedComponents = new List<Component>();

        if (dto.Components != null)
        {
            foreach (var c in dto.Components)
            {
                if (c.AltCompId.HasValue)
                {
                    var altComp = await _context.Components.FindAsync(c.AltCompId.Value)
                                  ?? throw new Exception("Alt component not found");
                    selectedComponents.Add(altComp);
                    addOnTotal += altComp.Price;
                }
            }
        }

        double amount = baseAmt + addOnTotal;
        double tax = amount * 0.18;
        double totalAmt = amount + tax;

        // Save Header
        var invoice = new InvoiceHeader
        {
            UserId = user.Id,
            ModelId = model.Id,
            Qty = dto.Qty.Value,
            BaseAmt = baseAmt,
            Tax = tax,
            TotalAmt = totalAmt,
            InvDate = DateOnly.FromDateTime(DateTime.Now),
            Status = InvoiceStatus.Confirmed,
            CustomerDetail = dto.CustomerDetail
        };

        _context.InvoiceHeaders.Add(invoice);
        await _context.SaveChangesAsync();

        // Save Details
        var savedDetails = new List<InvoiceDetail>();
        foreach (var comp in selectedComponents)
        {
            var d = new InvoiceDetail
            {
                InvId = invoice.Id, // FK set by EF after save? Yes.
                CompId = comp.CompId,
                CompPrice = comp.Price
            };
            _context.InvoiceDetails.Add(d);
            savedDetails.Add(d);
        }
        await _context.SaveChangesAsync(); // Save details

        foreach(var d in savedDetails) {
            if(d.Comp == null) d.Comp = selectedComponents.First(c => c.CompId == d.CompId);
        }
        // PDF & Email
        byte[] pdfBytes = PdfGeneratorUtil.GenerateInvoicePdf(invoice, savedDetails);
        
        _emailService.SendInvoiceEmail(user, pdfBytes, invoice.Id);
    }
}
