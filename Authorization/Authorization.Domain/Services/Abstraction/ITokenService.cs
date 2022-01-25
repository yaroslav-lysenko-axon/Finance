using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Authorization.Domain.Models;

namespace Authorization.Domain.Services.Abstraction
{
    public interface ITokenService
    {
        AccessToken IssueAccessToken(IUser user, List<string> scopeIds);
        Task<RefreshToken> IssueRefreshToken(User user, Client client);
        Task<RefreshToken> IssueRefreshToken(User user, Client client, DateTime createdAt, DateTime expireAt);
        Task<RefreshToken> GetRefreshToken(string refreshToken);
        Task LogoutRevokeRefreshTokens(User user, Client client);
    }
}
