using System;
using System.Linq;
using System.Threading.Tasks;
using Authorization.Domain.Models;
using Authorization.Domain.Repositories;
using Authorization.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Authorization.Infrastructure.Persistence.Repositories
{
    public class ConfirmationRequestRepository : GenericRepository<ConfirmationRequest>, IConfirmationRequestRepository
    {
        private readonly AuthContext _context;

        public ConfirmationRequestRepository(AuthContext context)
            : base(context.ConfirmationRequests)
        {
            _context = context;
        }

        public async Task<ConfirmationRequest> FindByConfirmationRequestId(Guid requestId)
        {
            var confirmationRequest = await _context.ConfirmationRequests
                .Include(confirmationRequest => confirmationRequest.User)
                .Where(confirmationRequest => confirmationRequest.Id == requestId)
                .FirstOrDefaultAsync().ConfigureAwait(false);
            return confirmationRequest;
        }

        public async Task RevokeConfirmationRequest(User user)
        {
            var now = DateTime.UtcNow;
            var existingConfirmationRequest = await FindFirst(confirmationRequest => confirmationRequest.User == user &&
                confirmationRequest.ExpiredAt > now &&
                confirmationRequest.RevokedAt == null);
            existingConfirmationRequest.RevokedAt = now;
            existingConfirmationRequest.Confirmed = true;
        }
    }
}
