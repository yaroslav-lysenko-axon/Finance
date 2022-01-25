using System;
using Authorization.Application.Extensions.Abstraction;
using Microsoft.AspNetCore.Http;

namespace Authorization.Application.Extensions
{
    public class CookiesExtensions : ICookiesExtensions
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public CookiesExtensions(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetTokenCookie(string refreshToken)
        {
            // append cookie with refresh token to the http response
            _httpContextAccessor?.HttpContext?.Response.Cookies.Append("refreshToken", refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddDays(7),
                    SameSite = SameSiteMode.Strict,
                    IsEssential = true,
                });
        }
    }
}
