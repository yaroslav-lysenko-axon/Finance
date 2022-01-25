using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Authorization.Domain.Models;

namespace Authorization.Domain.Services.Abstraction
{
    public interface IJwtService
    {
        JwtSecurityToken CreateJwt(IUser user, List<string> scopeIds);
        string GetJwtString(JwtSecurityToken token);
        bool ValidateToken(string token);
    }
}
