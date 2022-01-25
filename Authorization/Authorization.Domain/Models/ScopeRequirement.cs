using Microsoft.AspNetCore.Authorization;

namespace Authorization.Domain.Models
{
    public class ScopeRequirement : IAuthorizationRequirement
    {
        public ScopeRequirement(string scope)
        {
            Scope = scope;
        }

        public string Scope { get; set; }
    }
}
