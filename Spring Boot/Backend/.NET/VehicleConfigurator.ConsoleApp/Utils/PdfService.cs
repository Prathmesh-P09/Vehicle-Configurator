using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Utils
{
    public interface IPdfService
    {
        byte[] GenerateInvoicePdf(InvoiceHeader header, List<InvoiceDetail> details);
    }

    public class PdfService : IPdfService
    {
        public byte[] GenerateInvoicePdf(InvoiceHeader header, List<InvoiceDetail> details)
        {
            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            document.Add(new Paragraph("INVOICE").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20));
            document.Add(new Paragraph($"Invoice ID: {header.Id}"));
            document.Add(new Paragraph($"Date: {header.InvDate}"));
            document.Add(new Paragraph($"Customer: {header.User?.Username}"));
            document.Add(new Paragraph($"Model: {header.Model?.ModelName}"));
            document.Add(new Paragraph($"Quantity: {header.Qty}"));
            document.Add(new Paragraph("\n"));

            var table = new Table(3, true); // 3 columns
            table.AddHeaderCell("Component");
            table.AddHeaderCell("Type");
            table.AddHeaderCell("Price");

            foreach (var detail in details)
            {
                table.AddCell(detail.Comp?.CompName ?? "Unknown");
                table.AddCell(detail.Comp?.Type ?? "-");
                table.AddCell(detail.CompPrice.ToString("C"));
            }

            document.Add(table);

            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph($"Base Amount: {header.BaseAmt:C}"));
            document.Add(new Paragraph($"Tax: {header.Tax:C}"));
            document.Add(new Paragraph($"Total Amount: {header.TotalAmt:C}").SetBold());

            document.Close();
            return ms.ToArray();
        }
    }
}
