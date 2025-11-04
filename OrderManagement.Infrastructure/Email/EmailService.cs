using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace OrderManagement.Infrastructure.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOrderConfirmationEmailAsync(string toEmail, string customerName, Guid orderId, decimal totalAmount)
        {
            string smtpHost = _config["Email:SmtpHost"]!;
            int smtpPort = int.Parse(_config["Email:SmtpPort"]!);
            string fromEmail = _config["Email:FromEmail"]!;
            string fromName = _config["Email:FromName"]!;

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail, ""), 
                EnableSsl = false
            };

            string templatePath = Path.Combine(AppContext.BaseDirectory, "Email", "EmailTemplate.html");
            string htmlBody = await File.ReadAllTextAsync(templatePath);

            htmlBody = htmlBody
                .Replace("{{CustomerName}}", customerName)
                .Replace("{{OrderId}}", orderId.ToString())
                .Replace("{{TotalAmount}}", totalAmount.ToString("C"));

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = $"Order Confirmation #{orderId}",
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            await client.SendMailAsync(message);
        }
    }
}
