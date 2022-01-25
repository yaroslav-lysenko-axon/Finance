using System.Threading.Tasks;
using Authorization.Domain.Enums;
using Authorization.Domain.Services.Abstraction;

namespace Authorization.Domain.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly IConfirmationRequestService _confirmationRequestService;
        private readonly IUserService _userService;
        public PasswordService(
            IConfirmationRequestService confirmationRequestService,
            IUserService userService)
        {
            _confirmationRequestService = confirmationRequestService;
            _userService = userService;
        }

        public async Task SendPasswordRecoveryEmail(string email)
        {
            var user = await _userService.GetUser(email);
            await _confirmationRequestService.SendEmailConfirmationRequest(user, AdditionalSubject.PasswordRecovery, null);
        }
    }
}
