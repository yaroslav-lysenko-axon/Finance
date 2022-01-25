using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Authorization.Domain.Exceptions;
using Authorization.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Authorization.Domain.Handlers
{
    public class UserProfileHandler : AuthorizationHandler<UserProfileRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserProfileHandler(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// UserProfileHandler.
        /// </summary>
        /// <remarks>
        /// A handler for checking claims (id, name, surname, email and role) from user context.
        /// </remarks>
        protected override async Task<int> HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UserProfileRequirement requirement)
        {
            if (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true && _httpContextAccessor.HttpContext == null)
            {
                return (int)HttpStatusCode.Forbidden;
            }

            if (_httpContextAccessor.HttpContext == null)
            {
                return (int)HttpStatusCode.Unauthorized;
            }

            var user = _httpContextAccessor.HttpContext.User;
            var userContextRequirement = new UserProfileRequirement
            {
                Id = Guid.Parse(user.Claims.FirstOrDefault()?.Value ?? string.Empty),
                FirstName = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value,
                LastName = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Surname)?.Value,
                Email = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value,
                Role = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value,
            };

            var expiredLong = Convert.ToInt64(
                user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Expired)?.Value, CultureInfo.CurrentCulture);

            if (expiredLong < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                throw new AuthenticationException();
            }

            var authorizationHandlerContext =
                new AuthorizationHandlerContext(new[] { userContextRequirement }, user, null);

            if (!authorizationHandlerContext.User.HasClaim(claim => claim.Type == ClaimTypes.NameIdentifier)
                || !authorizationHandlerContext.User.HasClaim(claim => claim.Type == ClaimTypes.Name)
                || !authorizationHandlerContext.User.HasClaim(claim => claim.Type == ClaimTypes.Surname)
                || !authorizationHandlerContext.User.HasClaim(claim => claim.Type == ClaimTypes.Email)
                || !authorizationHandlerContext.User.HasClaim(claim => claim.Type == ClaimTypes.Role))
            {
                return (int)HttpStatusCode.Forbidden;
            }

            context.Succeed(requirement);
            await Task.CompletedTask;

            return (int)HttpStatusCode.OK;
        }
    }
}
