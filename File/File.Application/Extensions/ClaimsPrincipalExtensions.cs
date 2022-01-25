using System;
using System.Linq;
using System.Security.Claims;

namespace File.Application.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetAuthorizedUserId(this ClaimsPrincipal principal)
        {
            var userId = principal.Claims.FirstOrDefault(firstName => firstName.Type == ClaimTypes.NameIdentifier)?.Value;
            return userId != null ? Guid.Parse(userId) : Guid.Empty;
        }
    }
}
