using System.Threading.Tasks;

namespace Authorization.Domain.Services.Abstraction
{
    public interface IPasswordService
    {
        Task SendPasswordRecoveryEmail(string email);
    }
}
