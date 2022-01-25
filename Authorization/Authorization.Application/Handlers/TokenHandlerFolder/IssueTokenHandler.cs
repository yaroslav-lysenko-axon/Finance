using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Authorization.Application.Models.Commands.TokenCommands;
using Authorization.Application.Models.Responses;
using Authorization.Domain.ConfigurationClasses;
using Authorization.Domain.Repositories;
using Authorization.Domain.Services.Abstraction;
using MediatR;

namespace Authorization.Application.Handlers.TokenHandlerFolder
{
    public class IssueTokenHandler : IRequestHandler<IssueTokenCommand, TokenResponse>
    {
        private readonly IClientService _clientService;
        private readonly IUserService _userService;
        private readonly IRoleScopeService _roleScopeService;
        private readonly ITokenService _tokenService;
        private readonly ITokensConfiguration _tokensConfiguration;
        private readonly IUnitOfWork _unitOfWork;

        public IssueTokenHandler(
            IClientService clientService,
            IUserService userService,
            IRoleScopeService roleScopeService,
            ITokenService tokenService,
            ITokensConfiguration tokensConfiguration,
            IUnitOfWork unitOfWork)
        {
            _clientService = clientService;
            _userService = userService;
            _roleScopeService = roleScopeService;
            _tokenService = tokenService;
            _tokensConfiguration = tokensConfiguration;
            _unitOfWork = unitOfWork;
        }

        public async Task<TokenResponse> Handle(IssueTokenCommand request, CancellationToken cancellationToken)
        {
            var client = await _clientService.FindClient(request.ClientId);
            var user = await _userService.GetUser(request.Email, request.Password);
            var roleScopes = await _roleScopeService.GetRoleScope(user.Role.Id).ConfigureAwait(false);
            var scopeIds = roleScopes.Select(roleScope => roleScope.Scope.Id).ToList();
            var accessToken = _tokenService.IssueAccessToken(user, scopeIds);
            var refreshToken = _tokensConfiguration.ShouldIssueRefreshTokens
                ? await _tokenService.IssueRefreshToken(user, client)
                : null;

            if (refreshToken == null)
            {
                return null;
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
