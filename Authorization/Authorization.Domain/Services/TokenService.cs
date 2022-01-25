using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Authorization.Domain.ConfigurationClasses;
using Authorization.Domain.Enums;
using Authorization.Domain.Models;
using Authorization.Domain.Repositories;
using Authorization.Domain.Services.Abstraction;

namespace Authorization.Domain.Services
{
    public class TokenService : ITokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokensConfiguration _tokensConfiguration;
        private readonly ITimeProvider _timeProvider;
        private readonly IJwtService _jwtService;

        public TokenService(
            IRefreshTokenRepository refreshTokenRepository,
            ITokensConfiguration tokensConfiguration,
            ITimeProvider timeProvider,
            IJwtService jwtService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _tokensConfiguration = tokensConfiguration;
            _timeProvider = timeProvider;
            _jwtService = jwtService;
        }

        public AccessToken IssueAccessToken(IUser user, List<string> scopeIds)
        {
            const string tokenType = "bearer";

            var jwt = _jwtService.CreateJwt(user, scopeIds);
            var jwtString = _jwtService.GetJwtString(jwt);
            _jwtService.ValidateToken(jwtString);

            return new AccessToken
            {
                Token = jwtString,
                Type = tokenType,
            };
        }

        public async Task<RefreshToken> IssueRefreshToken(User user, Client client)
        {
            await _refreshTokenRepository.RevokeRefreshTokens(user, client, RefreshTokenRevokeReason.Refresh);
            var now = _timeProvider.UtcNow();
            var refreshToken = new RefreshToken
            {
                User = user,
                Client = client,
                CreatedAt = now,
                ExpireAt = now.AddHours(_tokensConfiguration.RefreshTokenExpirationTimeInHours),
                Token = Guid.NewGuid().ToString(),
            };

            await _refreshTokenRepository.Insert(refreshToken);

            return refreshToken;
        }

        public async Task<RefreshToken> IssueRefreshToken(User user, Client client, DateTime createdAt, DateTime expireAt)
        {
            await _refreshTokenRepository.RevokeRefreshTokens(user, client, RefreshTokenRevokeReason.Refresh);
            var refreshToken = new RefreshToken
            {
                User = user,
                Client = client,
                CreatedAt = createdAt,
                ExpireAt = expireAt,
                Token = Guid.NewGuid().ToString(),
            };

            await _refreshTokenRepository.Insert(refreshToken);

            return refreshToken;
        }

        public async Task<RefreshToken> GetRefreshToken(string refreshToken)
        {
            var refreshTokenInfo = await _refreshTokenRepository.GetRefreshToken(refreshToken);
            return refreshTokenInfo;
        }

        public async Task LogoutRevokeRefreshTokens(User user, Client client)
        {
            await _refreshTokenRepository.RevokeRefreshTokens(user, client, RefreshTokenRevokeReason.Logout);
        }
    }
}
