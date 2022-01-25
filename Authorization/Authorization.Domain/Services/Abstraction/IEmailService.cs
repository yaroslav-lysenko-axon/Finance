using System.Threading.Tasks;
using Authorization.Domain.Models;

namespace Authorization.Domain.Services.Abstraction
{
    public interface IEmailService
    {
        Task SendEmail(EmailData emailData);
    }
}
