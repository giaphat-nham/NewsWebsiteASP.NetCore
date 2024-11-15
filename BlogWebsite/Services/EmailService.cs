using Microsoft.Extensions.Options;
using NewsWebsite.Models;
using System.Net.Mail;
using System.Net;

namespace NewsWebsite.Services
{
    public interface IEmailService { 
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
            {
                client.Credentials = new NetworkCredential(_smtpSettings.SenderEmail, _smtpSettings.Password);
                client.EnableSsl = _smtpSettings.UseSSL;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.SenderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
