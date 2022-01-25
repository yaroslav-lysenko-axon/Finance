using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Authorization.Application.Models.Commands.RegistrationCommands;
using Authorization.Application.Models.Responses;
using Authorization.Domain.ConfigurationClasses;
using Authorization.Domain.Enums;
using Authorization.Domain.Repositories;
using Authorization.Domain.Services.Abstraction;
using MediatR;

namespace Authorization.Application.Handlers.RegistrationHandlerFolder
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, TokenResponse>
    {
        private readonly IClientService _clientService;
        private readonly IUserService _userService;
        private readonly IRoleScopeService _roleScopeService;
        private readonly IConfirmationRequestService _confirmationRequestService;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokensConfiguration _tokensConfiguration;

        public RegisterUserHandler(
            IClientService clientService,
            IUserService userService,
            IRoleScopeService roleScopeService,
            IConfirmationRequestService confirmationRequestService,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            ITokensConfiguration tokensConfiguration)
        {
            _clientService = clientService;
            _userService = userService;
            _roleScopeService = roleScopeService;
            _confirmationRequestService = confirmationRequestService;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _tokensConfiguration = tokensConfiguration;
        }

        public async Task<TokenResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var client = await _clientService.AuthenticateClient(request.ClientId, request.ClientSecret);
            var user = await _userService.RegisterUser(request.Email, request.Password, request.FirstName, request.LastName);
            var roleScopes = await _roleScopeService.GetRoleScope(user.Role.Id).ConfigureAwait(false);
            var scopeIds = roleScopes.Select(roleScope => roleScope.Scope.Id).ToList();
            var accessToken = _tokenService.IssueAccessToken(user, scopeIds);
            var refreshToken = _tokensConfiguration.ShouldIssueRefreshTokens
                ? await _tokenService.IssueRefreshToken(user, client)
                : null;

            await _confirmationRequestService.SendEmailConfirmationRequest(user, AdditionalSubject.Registration, null);

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
