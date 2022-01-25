using System;
using System.Linq;
using System.Threading.Tasks;
using Authorization.Domain.Enums;
using Authorization.Domain.Models;
using Authorization.Domain.Repositories;
using Authorization.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Authorization.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly AuthContext _context;
        public RefreshTokenRepository(AuthContext context)
            : base(context.RefreshTokens)
        {
            _context = context;
        }

        public async Task RevokeRefreshTokens(User user, Client client, RefreshTokenRevokeReason reason)
        {
            var now = DateTime.UtcNow;
            var existingRefreshTokens = await Find(x => x.User == user &&
                                                        x.Client == client &&
                                                        x.ExpireAt > now &&
                                                        x.RevokedAt == null);

            foreach (var existingRefreshToken in existingRefreshTokens)
            {
                existingRefreshToken.RevokedAt = now;
                existingRefreshToken.RevokeReason = reason.ToString();
            }
        }

        public async Task<RefreshToken> GetRefreshToken(string refreshTokenKey)
        {
            var refreshToken = await _context.RefreshTokens
                .Include(refreshToken => refreshToken.Client)
                .Include(refreshToken => refreshToken.User)
                .Where(refreshToken => refreshToken.Token == refreshTokenKey)
                .FirstOrDefaultAsync().ConfigureAwait(false);
            return refreshToken;
        }
    }
}
