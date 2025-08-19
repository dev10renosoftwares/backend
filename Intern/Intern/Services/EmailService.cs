using System.Net.Mail;
using System.Net;

namespace Intern.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration configuration)
        {
            _config = configuration;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var fromAddress = _config["EmailSettings:FromAddress"];
            var fromPassword = _config["EmailSettings:Password"];
            var smtpHost = _config["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_config["EmailSettings:Port"]);

            using (var smtp = new SmtpClient(smtpHost, smtpPort))
            {
                smtp.Credentials = new NetworkCredential(fromAddress, fromPassword);
                smtp.EnableSsl = bool.Parse(_config["EmailSettings:UseSsl"]);

                var mail = new MailMessage
                {
                    From = new MailAddress(fromAddress, _config["EmailSettings:FromName"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add(to);

                await smtp.SendMailAsync(mail);
            }
        }
    }
}
