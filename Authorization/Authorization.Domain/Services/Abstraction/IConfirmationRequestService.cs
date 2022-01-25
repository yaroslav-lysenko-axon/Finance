using System;
using System.Threading.Tasks;
using Authorization.Domain.Enums;
using Authorization.Domain.Models;

namespace Authorization.Domain.Services.Abstraction
{
    public interface IConfirmationRequestService
    {
        Task<ConfirmationRequest> ConfirmationRequest(Guid requestId, string requestHash);
        Task SendEmailConfirmationRequest(User user, AdditionalSubject subject, ConfirmationRequestRevokeReason? revokeReason);
    }
}
