using System;
using System.Collections.Generic;
using Authorization.Domain.Enums;
using SendGrid.Helpers.Mail;

namespace Authorization.Domain.Models
{
    public class EmailData
    {
        public EmailAddress Sender { get; set; }
        public List<EmailAddress> Recipients { get; set; }
        public AdditionalSubject AdditionalSubject { get; set; }
        public EmailTemplate EmailTemplate { get; set; }
        public Uri UriLink { get; set; }
    }
}
