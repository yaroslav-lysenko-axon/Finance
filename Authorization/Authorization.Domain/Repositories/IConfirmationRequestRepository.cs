using System;
using System.Threading.Tasks;
using Authorization.Domain.Models;

namespace Authorization.Domain.Repositories
{
    public interface IConfirmationRequestRepository : IGenericRepository<ConfirmationRequest>
    {
        Task<ConfirmationRequest> FindByConfirmationRequestId(Guid requestId);
        Task RevokeConfirmationRequest(User user);
    }
}
