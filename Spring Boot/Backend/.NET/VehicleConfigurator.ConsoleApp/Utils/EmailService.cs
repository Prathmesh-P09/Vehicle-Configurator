using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace VehicleConfigurator.ConsoleApp.Utils
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, byte[]? attachment = null, string? attachmentName = null);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body, byte[]? attachment = null, string? attachmentName = null)
        {
            try
            {
                var host = _config["Email:Host"];
                var port = int.Parse(_config["Email:Port"] ?? "587");
                var username = _config["Email:Username"];
                var password = _config["Email:Password"];
                var enableSsl = bool.Parse(_config["Email:EnableSsl"] ?? "true");

                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = enableSsl
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(username!),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                if (attachment != null && !string.IsNullOrEmpty(attachmentName))
                {
                    mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachment), attachmentName));
                }

                await client.SendMailAsync(mailMessage);
                Console.WriteLine($"[EmailService] Email sent to {to} successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService] Failed to send email: {ex.Message}");
                // Not throwing to avoid breaking the flow, mimicking 'try-catch' in typical service logic
            }
        }
    }
}
