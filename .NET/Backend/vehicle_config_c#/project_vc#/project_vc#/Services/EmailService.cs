using MimeKit;
using MailKit.Net.Smtp;
using project_vc_.Models;
using System.IO;

namespace project_vc_.Services;

public interface IEmailService
{
    void SendRegistrationEmail(User user, byte[] pdfBytes);
    void SendInvoiceEmail(User user, byte[] pdfBytes, int invoiceId);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public void SendRegistrationEmail(User user, byte[] pdfBytes)
    {
        string subject = "Registration Successful - PDF Attached";
        string messageBody =
                $"Hello {user.AuthName},\n\n" +
                "Your registration has been completed successfully.\n\n" +
                $"Registration No: {user.RegistrationNo}\n" +
                $"Company: {user.CompanyName}\n\n" +
                "Please find your registration PDF attached.\n\n" +
                "Regards,\nVehicle Configurator Team";

        SendEmailWithAttachment(user.Email!, subject, messageBody, pdfBytes, "registration-details.pdf");
    }

    public void SendInvoiceEmail(User user, byte[] pdfBytes, int invoiceId)
    {
        _logger.LogInformation($"Sending Invoice Email to {user.Email}, Invoice ID: {invoiceId}");

        string subject = $"Invoice #{invoiceId}";
        string messageBody =
                $"Hello {user.AuthName},\n\n" +
                "Thank you for your vehicle order.\n\n" +
                $"Invoice No: {invoiceId}\n\n" +
                "Please find the invoice PDF attached.\n\n" +
                "Regards,\nVehicle Configurator Team";

        SendEmailWithAttachment(user.Email!, subject, messageBody, pdfBytes, $"invoice_{invoiceId}.pdf");
    }

    private void SendEmailWithAttachment(string toEmail, string subject, string body, byte[] attachment, string filename)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Vehicle Configurator", _config["Email:From"] ?? "no-reply@vconf.com"));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder { TextBody = body };
            builder.Attachments.Add(filename, attachment);

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            // Connect to SMTP server
            client.Connect(_config["Email:Host"], int.Parse(_config["Email:Port"]!), MailKit.Security.SecureSocketOptions.StartTls);
            
            // Authenticate
            client.Authenticate(_config["Email:Username"], _config["Email:Password"]);
            
            // Send
            client.Send(message);
            client.Disconnect(true);
            
            _logger.LogInformation($"[Email Sent] To: {toEmail}, Subject: {subject}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email");
        }
    }
}
