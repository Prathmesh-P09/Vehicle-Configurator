using VehicleConfigurator.ConsoleApp.Data.Repositories;
using VehicleConfigurator.ConsoleApp.DTOs;
using VehicleConfigurator.ConsoleApp.Models;
using VehicleConfigurator.ConsoleApp.Utils;

namespace VehicleConfigurator.ConsoleApp.Services
{
    public class InvoiceManager : IInvoiceManager
    {
        private readonly IInvoiceHeaderRepository _invoiceRepo;
        private readonly IInvoiceDetailRepository _invoiceDetailRepo;
        private readonly IModelRepository _modelRepo;
        private readonly IUserRepository _userRepo;
        private readonly IAlternateComponentRepository _altCompRepo;
        private readonly IComponentRepository _compRepo;
        private readonly IPdfService _pdfService;
        private readonly IEmailService _emailService;

        public InvoiceManager(IInvoiceHeaderRepository invoiceRepo,
                              IInvoiceDetailRepository invoiceDetailRepo,
                              IModelRepository modelRepo,
                              IUserRepository userRepo,
                              IAlternateComponentRepository altCompRepo,
                              IComponentRepository compRepo,
                              IPdfService pdfService,
                              IEmailService emailService)
        {
            _invoiceRepo = invoiceRepo;
            _invoiceDetailRepo = invoiceDetailRepo;
            _modelRepo = modelRepo;
            _userRepo = userRepo;
            _altCompRepo = altCompRepo;
            _compRepo = compRepo;
            _pdfService = pdfService;
            _emailService = emailService;
        }

        public async Task<InvoiceResponseDto> GenerateInvoiceAsync(InvoiceRequestDto request)
        {
            // 1. Fetch User
            var user = await _userRepo.FindByUsernameAsync(request.Username);
            if (user == null) throw new Exception("User not found");

            // 2. Fetch Model
            var model = await _modelRepo.FindByIdAsync(request.ModelId);
            if (model == null) throw new Exception("Model not found");

            // 3. Calculate Base Amount
            double baseAmount = model.Price * request.Qty;
            double addOnTotal = 0;
            var invoiceDetails = new List<InvoiceDetail>();

            // 4. Process Components
            var allAltRules = await _altCompRepo.FindByModelIdAsync(request.ModelId);

            foreach (var compReq in request.Components)
            {
                // Find rule for this alternate
                var rule = allAltRules.FirstOrDefault(a => a.CompId == compReq.CompId && a.AltCompId == compReq.AltCompId);
                
                double delta = 0;
                double itemPrice = 0;

                if (rule != null)
                {
                    delta = rule.DeltaPrice;
                    // If fetching exact price of component is needed, fetch it.
                    // But Logic usually is: ModelPrice includes Base Components.
                    // Alternate adds Delta.
                }

                addOnTotal += delta;

                // Create Detail Line
                var detail = new InvoiceDetail
                {
                    CompId = compReq.AltCompId,
                    CompPrice = delta // Storing Delta as price in detail? Or full price? 
                                      // Typically Invoice Detail shows the Item Price. 
                                      // If Model Price is Base, then Alternate Price might be BaseCompPrice + Delta?
                                      // Provided logic: "Calculate base amount... Add alternate component prices".
                                      // I will store Delta for now as "CompPrice" in InvoiceDetail to match calculation.
                };
                invoiceDetails.Add(detail);
            }

            // 5. Taxes & Totals
            double totalBeforeTax = baseAmount + addOnTotal;
            double tax = totalBeforeTax * 0.05; // 5% Tax? Java code said 18% in plan, but let's check code or plan. 
                                                // Plan said "18%". I will use 0.18.
                                                // Wait, Java Code `InvoiceManager.java` calculation:
                                                // `total = base + addOn`. `tax = total * 0.18`. `final = total + tax`.
            tax = totalBeforeTax * 0.18;
            double finalAmount = totalBeforeTax + tax;

            // 6. Create Invoice Header
            var invoice = new InvoiceHeader
            {
                UserId = user.Id,
                ModelId = model.Id,
                Qty = request.Qty,
                BaseAmt = baseAmount, // Only Model * Qty? Or Include AddOns? 
                                      // Usually BaseAmt = Model * Qty.
                TotalAmt = finalAmount,
                Tax = tax,
                InvDate = DateOnly.FromDateTime(DateTime.Now),
                Status = InvoiceStatus.Confirmed,
                CustomerDetail = "Generated via Console"
            };

            // 7. Persist
            invoice = await _invoiceRepo.SaveAsync(invoice);

            foreach (var d in invoiceDetails)
            {
                d.InvId = invoice.Id;
                await _invoiceDetailRepo.SaveAsync(d);
            }

            // 8. Generate PDF
            // Need to reload Invoice with navigation properties for proper PDF generation
            // Or manually set them for the PDF call
            invoice.User = user;
            invoice.Model = model;
            // Fetch Component names for details
            foreach (var d in invoiceDetails)
            {
                d.Comp = await _compRepo.FindByIdAsync(d.CompId);
            }

            byte[] pdfBytes = _pdfService.GenerateInvoicePdf(invoice, invoiceDetails);
            string pdfPath = Path.Combine("C:/logs", $"Invoice_{invoice.Id}.pdf"); // Saving locally as side effect
            if (!Directory.Exists("C:/logs")) Directory.CreateDirectory("C:/logs");
            await File.WriteAllBytesAsync(pdfPath, pdfBytes);

            // 9. Email
            await _emailService.SendEmailAsync(user.Email, $"Invoice #{invoice.Id} Confirmed", "Please find attached your invoice.", pdfBytes, $"Invoice_{invoice.Id}.pdf");

            return new InvoiceResponseDto
            {
                InvoiceId = invoice.Id,
                TotalAmount = finalAmount,
                PdfPath = pdfPath
            };
        }
    }
}
