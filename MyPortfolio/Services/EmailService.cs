using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace MyPortfolio.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var host = _configuration["SmtpSettings:Host"];
            var port = int.Parse(_configuration["SmtpSettings:Port"]);
            var email = _configuration["SmtpSettings:Email"];
            var password = _configuration["SmtpSettings:Password"];
            var displayName = _configuration["SmtpSettings:DisplayName"];

            _logger.LogInformation("SMTP Baglanti: {Host}:{Port}, From: {Email}, To: {To}", host, port, email, toEmail);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(displayName, email));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using var client = new SmtpClient();

            try
            {
                // Port 587 = STARTTLS, Port 465 = SSL
                await client.ConnectAsync(host, port, SecureSocketOptions.Auto);
                _logger.LogInformation("SMTP baglanti basarili");

                await client.AuthenticateAsync(email, password);
                _logger.LogInformation("SMTP kimlik dogrulama basarili");

                await client.SendAsync(message);
                _logger.LogInformation("E-posta basariyla gonderildi: {To}", toEmail);

                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMTP Hatasi: {Message}", ex.Message);
                throw;
            }
        }
    }
}
