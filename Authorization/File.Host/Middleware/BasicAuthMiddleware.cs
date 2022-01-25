using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace File.Host.Middleware
{
   public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// BasicAuthMiddleware.
        /// </summary>
        /// <remarks>
        /// Middleware for adding a claims principal to the user's context.
        /// </remarks>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            string authHeader = httpContext.Request.Headers["Authorization"];
            if (authHeader != null)
            {
                var jwt = authHeader.Split(new char[] { ' ' })[1];
                var handler = new JwtSecurityTokenHandler();
                if (handler.ReadToken(jwt) is JwtSecurityToken securityToken)
                {
                    var scopes = securityToken.Claims.Where(claim => claim.Type == "scopes").ToList();
                    var claimCollection = new List<Claim>
                    {
                        new (ClaimTypes.NameIdentifier, securityToken.Claims.First(claim => claim.Type == "nameid").Value),
                        new (ClaimTypes.Name, securityToken.Claims.First(claim => claim.Type == "unique_name").Value),
                        new (ClaimTypes.Surname, securityToken.Claims.First(claim => claim.Type == "family_name").Value),
                        new (ClaimTypes.Email, securityToken.Claims.First(claim => claim.Type == "email").Value),
                        new (ClaimTypes.Role, securityToken.Claims.First(claim => claim.Type == "role").Value),
                        new (ClaimTypes.Expired, securityToken.Claims.First(claim => claim.Type == "exp").Value),
                    };
                    claimCollection.AddRange(scopes);

                    var claimsIdentity = new ClaimsIdentity(claimCollection, "Profile");
                    var principal = new ClaimsPrincipal(claimsIdentity);

                    httpContext.User = principal;

                    await _next(httpContext);
                }
                else
                {
                    httpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                }
            }
            else
            {
                await _next(httpContext);
            }
        }
    }
}
