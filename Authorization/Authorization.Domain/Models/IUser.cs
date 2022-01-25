using System;
using System.Collections.Generic;

namespace Authorization.Domain.Models
{
    public interface IUser
    {
        public Guid Id { get; }
        public string Email { get; }
        public string PasswordHash { get; }
        public Guid Avatar { get; }
        public string Salt { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public Role Role { get; }
        public bool Active { get; }
        public DateTime CreatedAt { get; }
        public DateTime? RemovedAt { get; }
        public ICollection<ConfirmationRequest> ConfirmationRequests { get; }
    }
}
