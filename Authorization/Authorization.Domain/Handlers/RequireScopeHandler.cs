using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Authorization.Domain.Models;
using Authorization.Domain.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Authorization.Domain.Handlers
{
    public class RequireScopeHandler : AuthorizationHandler<ScopeRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IRoleScopeService _roleScopeService;

        public RequireScopeHandler(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IRoleScopeService roleScopeService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _roleScopeService = roleScopeService;
        }

        /// <summary>
        /// RequireScopeHandler.
        /// </summary>
        /// <remarks>
        /// A handler for checking scopes from user context.
        /// </remarks>
        protected override async Task<int> HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ScopeRequirement requirement)
        {
            if (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true && _httpContextAccessor.HttpContext == null)
            {
                return (int)HttpStatusCode.Forbidden;
            }

            if (_httpContextAccessor.HttpContext == null)
            {
                return (int)HttpStatusCode.Unauthorized;
            }

            var userEmail = _httpContextAccessor.HttpContext?.User.Claims
                .Where(claim => claim.Type == ClaimTypes.Email)
                .Select(claim => claim.Value)
                .FirstOrDefault();
            var user = await _userService.GetUser(userEmail);
            var scopes = await _roleScopeService.GetRoleScope(user.Role.Id);
            foreach (var scope in scopes)
            {
                if (scope.ScopeId != requirement.Scope)
                {
                    continue;
                }

                var scopeRequirement = new ScopeRequirement(scope.ScopeId)
                {
                    Scope = _httpContextAccessor.HttpContext?.User.Claims
                        .FirstOrDefault()?.Value,
                };
                var authorizationHandlerContext =
                    new AuthorizationHandlerContext(
                        new[] { scopeRequirement },
                        _httpContextAccessor.HttpContext.User,
                        null);
                var scopeClaim = authorizationHandlerContext.User.FindFirst(claim =>
                    claim.Type == "scopes" && claim.Value == scope.ScopeId);
                if (requirement.Scope != scope.ScopeId || scopeClaim == null || string.IsNullOrEmpty(scopeClaim.Value))
                {
                    await Task.CompletedTask;
                    return _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                }

                context.Succeed(requirement);
                await Task.FromResult(0);
            }

            return (int)HttpStatusCode.OK;
        }
    }
}
