using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace Application.Common.Abstractions
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string subject, string content);
    }

    public class SendGridMailService : IMailService
    {

        private IConfiguration _configuration;

        public SendGridMailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            var apiKey = _configuration["SendGridAPIKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("SendGridAPIKey is not configured or is null.");
            }
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("gentuarlushtaku2002@gmail.com", "GentuarLushtaku");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            await client.SendEmailAsync(msg);
        }
    }

}
