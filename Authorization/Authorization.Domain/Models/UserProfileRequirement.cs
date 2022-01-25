using System;
using Microsoft.AspNetCore.Authorization;

namespace Authorization.Domain.Models
{
    public class UserProfileRequirement : IAuthorizationRequirement
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
