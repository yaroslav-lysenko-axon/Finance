using System.Linq;
using System.Threading.Tasks;
using Authorization.Domain.ConfigurationClasses;
using Authorization.Domain.Models;
using Authorization.Domain.Services.Abstraction;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Authorization.Domain.Services
{
    public class EmailService : IEmailService
    {
        private readonly ISendGridConfiguration _sendGridConfiguration;
        public EmailService(
            ISendGridConfiguration sendGridConfiguration)
        {
            _sendGridConfiguration = sendGridConfiguration;
        }

        public async Task SendEmail(EmailData emailData)
        {
            var sendGridClient = new SendGridClient(_sendGridConfiguration.ApiKey);
            var from = new EmailAddress(emailData.Sender.Email, emailData.Sender.Name);
            var subject = emailData.AdditionalSubject.ToString();
            var recipients = emailData.Recipients.FirstOrDefault();
            var to = new EmailAddress(recipients?.Email, recipients?.Name);
            var plainContent = "Hello";
            var htmlContent = $"<h1>Please, confirm {subject} by link: {emailData.UriLink}</h1>";
            var mailMessage = MailHelper.CreateSingleEmail(from, to, subject, plainContent, htmlContent);
            await sendGridClient.SendEmailAsync(mailMessage);
        }
    }
}
