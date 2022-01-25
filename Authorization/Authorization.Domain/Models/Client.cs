using System;

namespace Authorization.Domain.Models
{
    public class Client
    {
        public Guid Id { get; set; }
        public Guid ClientSecret { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
