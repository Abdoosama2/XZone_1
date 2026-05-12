using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using XZone.Application.Services.IServices;

namespace XZone.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IConfiguration config,
            ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<bool> SendMessage(string email, string subject, string message)
        {
            try
            {
                // ✅ Read from appsettings.json — no hardcoded values
                var host = _config["EmailSettings:Host"];
                var port = int.Parse(_config["EmailSettings:Port"]);
                var senderEmail = _config["EmailSettings:SenderEmail"];
                var senderName = _config["EmailSettings:SenderName"];
                var appPassword = _config["EmailSettings:AppPassword"];

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = message,
                    TextBody = "Please view this email in an HTML-supported client."
                };

                var myMessage = new MimeMessage();
                myMessage.From.Add(new MailboxAddress(senderName, senderEmail));
                myMessage.To.Add(new MailboxAddress(email, email)); 
                myMessage.Subject = subject;
                myMessage.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(senderEmail, appPassword);
                await client.SendAsync(myMessage);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", email); // ✅ proper logging
                return false;
            }
        }
    }
}