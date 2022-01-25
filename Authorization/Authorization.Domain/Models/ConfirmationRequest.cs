using System;

namespace Authorization.Domain.Models
{
    public class ConfirmationRequest
    {
        public Guid Id { get; set; }
        public User User { get; set; }

        public string Subject { get; set; }
        public string AdditionalSubject { get; set; }
        public bool Confirmed { get; set; }
        public string RequestType { get; set; }
        public string Receiver { get; set; }
        public DateTime? RevokedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiredAt { get; set; }

        public int GetRequestHashCode()
        {
            return HashCode.Combine(Id, User.Id);
        }
    }
}
