using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Authorization.Application.Models.Commands.TokenCommands;
using Authorization.Domain.Repositories;
using Authorization.Domain.Services.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Application.Handlers.TokenHandlerFolder
{
    public class LogoutHandler : IRequestHandler<LogoutCommand, OkResult>
    {
        private readonly IUserService _userService;
        private readonly IRoleScopeService _roleScopeService;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public LogoutHandler(
            IUserService userService,
            IRoleScopeService roleScopeService,
            ITokenService tokenService,
            IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _roleScopeService = roleScopeService;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<OkResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var refreshTokenInfo = await _tokenService.GetRefreshToken(request.RefreshToken);
            var user = await _userService.GetUser(refreshTokenInfo.User.Email);
            var roleScopes = await _roleScopeService.GetRoleScope(user.Role.Id).ConfigureAwait(false);
            var scopeIds = roleScopes.Select(roleScope => roleScope.Scope.Id).ToList();
            var accessToken = _tokenService.IssueAccessToken(user, scopeIds);
            if (accessToken.Token != null)
            {
                await _tokenService.LogoutRevokeRefreshTokens(refreshTokenInfo.User, refreshTokenInfo.Client);
            }

            await _unitOfWork.Commit();
            return new OkResult();
        }
    }
}
