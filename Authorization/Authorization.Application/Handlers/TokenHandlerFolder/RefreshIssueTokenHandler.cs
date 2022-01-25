using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Authorization.Application.Models.Commands.TokenCommands;
using Authorization.Application.Models.Responses;
using Authorization.Domain.ConfigurationClasses;
using Authorization.Domain.Exceptions;
using Authorization.Domain.Repositories;
using Authorization.Domain.Services.Abstraction;
using MediatR;

namespace Authorization.Application.Handlers.TokenHandlerFolder
{
    public class RefreshIssueTokenHandler : IRequestHandler<RefreshTokenCommand, TokenResponse>
    {
        private readonly ITokenService _tokenService;
        private readonly ITokensConfiguration _tokensConfiguration;
        private readonly ITimeProvider _timeProvider;
        private readonly IUserService _userService;
        private readonly IRoleScopeService _roleScopeService;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshIssueTokenHandler(
            ITokenService tokenService,
            ITokensConfiguration tokensConfiguration,
            ITimeProvider timeProvider,
            IUserService userService,
            IRoleScopeService roleScopeService,
            IUnitOfWork unitOfWork)
        {
            _tokenService = tokenService;
            _tokensConfiguration = tokensConfiguration;
            _timeProvider = timeProvider;
            _userService = userService;
            _roleScopeService = roleScopeService;
            _unitOfWork = unitOfWork;
        }

        public async Task<TokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshTokenInfo = await _tokenService.GetRefreshToken(request.RefreshToken);
            var user = await _userService.GetUser(refreshTokenInfo.User.Email);
            var roleScopes = await _roleScopeService.GetRoleScope(user.Role.Id).ConfigureAwait(false);
            var scopeIds = roleScopes.Select(roleScope => roleScope.Scope.Id).ToList();
            var accessToken = _tokenService.IssueAccessToken(user, scopeIds);
            var refreshToken = _tokensConfiguration.ShouldIssueRefreshTokens
                ? await _tokenService.IssueRefreshToken(
                    refreshTokenInfo.User,
                    refreshTokenInfo.Client,
                    refreshTokenInfo.CreatedAt,
                    refreshTokenInfo.ExpireAt)
                : null;

            if (refreshToken == null)
            {
                return null;
            }

            if (refreshToken.ExpireAt < _timeProvider.UtcNow())
            {
                throw new AuthenticationException();
            }

            var response = new TokenResponse
            {
                AccessTokenResponse = new AccessTokenResponse
                {
                    AccessToken = accessToken.Token,
                    TokenType = accessToken.Type,
                },
                RefreshTokenResponse = new RefreshTokenResponse
                {
                    RefreshToken = refreshToken.Token,
                },
            };
            await _unitOfWork.Commit();
            return response;
        }
    }
}
