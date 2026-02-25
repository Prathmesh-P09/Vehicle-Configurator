using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using project_vc_.Models;

namespace project_vc_.Utils;

public class PdfGeneratorUtil
{
    public static byte[] GenerateUserPdf(User user)
    {
        using var ms = new MemoryStream();
        var writer = new PdfWriter(ms);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        // Fonts in iText7 (StandardFonts)
        var titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        var normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

        // Title
        document.Add(new Paragraph("User Registration Details")
            .SetFont(titleFont).SetFontSize(18));
        
        document.Add(new Paragraph("\n")); // Space

        // Company Details
        document.Add(new Paragraph("Company Details").SetFont(boldFont).SetFontSize(14));
        
        AddField(document, boldFont, normalFont, "Company Name", user.CompanyName);
        AddField(document, boldFont, normalFont, "Holding Type", user.HoldingType);
        AddField(document, boldFont, normalFont, "Company ST No", user.CompanyStNo);
        AddField(document, boldFont, normalFont, "Company VAT No", user.CompanyVatNo);
        AddField(document, boldFont, normalFont, "PAN Number", user.TaxPan);
        AddField(document, boldFont, normalFont, "Registration No", user.RegistrationNo);

        document.Add(new Paragraph("\n"));

        // Auth Person
        document.Add(new Paragraph("Authorized Person Details").SetFont(boldFont).SetFontSize(14));
        AddField(document, boldFont, normalFont, "Name", user.AuthName);
        AddField(document, boldFont, normalFont, "Designation", user.Designation);
        AddField(document, boldFont, normalFont, "Auth Telephone", user.AuthTel);
        AddField(document, boldFont, normalFont, "Mobile", user.Cell);
        AddField(document, boldFont, normalFont, "Phone", user.Phone);
        AddField(document, boldFont, normalFont, "Email", user.Email);

        document.Add(new Paragraph("\n"));

        // Address
        document.Add(new Paragraph("Address Details").SetFont(boldFont).SetFontSize(14));
        AddField(document, boldFont, normalFont, "Address Line 1", user.Add1);
        AddField(document, boldFont, normalFont, "Address Line 2", user.Add2);
        AddField(document, boldFont, normalFont, "City", user.City);
        AddField(document, boldFont, normalFont, "State", user.State);
        AddField(document, boldFont, normalFont, "PIN Code", user.Pin);
        AddField(document, boldFont, normalFont, "Telephone", user.Tel);
        AddField(document, boldFont, normalFont, "Fax", user.Fax);

        document.Add(new Paragraph("\n"));

        // Account
        document.Add(new Paragraph("Account Details").SetFont(boldFont).SetFontSize(14));
        AddField(document, boldFont, normalFont, "Username", user.Username);
        AddField(document, boldFont, normalFont, "Role", user.Role);

        document.Close();
        return ms.ToArray();
    }

    private static void AddField(Document document, PdfFont labelFont, PdfFont valueFont, string label, string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;

        var p = new Paragraph();
        p.Add(new Text(label + ": ").SetFont(labelFont).SetFontSize(11));
        p.Add(new Text(value).SetFont(valueFont).SetFontSize(11));
        document.Add(p);
    }

    public static byte[] GenerateInvoicePdf(InvoiceHeader invoice, List<InvoiceDetail> details)
    {
        using var ms = new MemoryStream();
        var writer = new PdfWriter(ms);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        // Fonts
        var titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        var normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

        document.Add(new Paragraph("VEHICLE INVOICE").SetFont(titleFont).SetFontSize(18));
        document.Add(new Paragraph($"Invoice ID: {invoice.Id}").SetFont(normalFont));
        document.Add(new Paragraph($"Date: {invoice.InvDate}").SetFont(normalFont));
        document.Add(new Paragraph($"Customer: {invoice.CustomerDetail}").SetFont(normalFont));
        document.Add(new Paragraph("\n"));

        // Table
        var table = new Table(3).UseAllAvailableWidth();
        table.AddHeaderCell(new Cell().Add(new Paragraph("Component").SetFont(boldFont)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Type").SetFont(boldFont)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Price").SetFont(boldFont)));

        foreach (var d in details)
        {
            table.AddCell(new Paragraph(d.Comp?.CompName ?? "Unknown").SetFont(normalFont));
            table.AddCell(new Paragraph(d.Comp?.Type ?? "-").SetFont(normalFont));
            table.AddCell(new Paragraph($"INR {d.CompPrice}").SetFont(normalFont));
        }

        document.Add(table);
        document.Add(new Paragraph("\n"));
        
        document.Add(new Paragraph($"Base Amount: INR {invoice.BaseAmt}").SetFont(normalFont));
        document.Add(new Paragraph($"Tax: INR {invoice.Tax}").SetFont(normalFont));
        document.Add(new Paragraph($"Total: INR {invoice.TotalAmt}").SetFont(boldFont));

        document.Close();
        return ms.ToArray();
    }
}
