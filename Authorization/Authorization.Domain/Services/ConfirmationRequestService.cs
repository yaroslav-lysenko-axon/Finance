using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Authorization.Domain.ConfigurationClasses;
using Authorization.Domain.Enums;
using Authorization.Domain.Exceptions;
using Authorization.Domain.Models;
using Authorization.Domain.Repositories;
using Authorization.Domain.Services.Abstraction;
using SendGrid.Helpers.Mail;

namespace Authorization.Domain.Services
{
    public class ConfirmationRequestService : IConfirmationRequestService
    {
        private const string UserConfirmationRole = "USER";
        private const string RequestIdParam = "id";
        private const string HashParam = "hash";
        private readonly IConfirmationRequestRepository _confirmationRequestRepository;
        private readonly IEmailConfiguration _emailConfiguration;
        private readonly ISendGridConfiguration _sendGridConfiguration;
        private readonly IRoleService _roleService;
        private readonly IEmailService _emailService;
        private readonly ITimeProvider _timeProvider;

        public ConfirmationRequestService(
            IConfirmationRequestRepository confirmationRequestRepository,
            IEmailConfiguration emailConfiguration,
            ISendGridConfiguration sendGridConfiguration,
            IRoleService roleService,
            IEmailService emailService,
            ITimeProvider timeProvider)
        {
            _confirmationRequestRepository = confirmationRequestRepository;
            _emailConfiguration = emailConfiguration;
            _sendGridConfiguration = sendGridConfiguration;
            _roleService = roleService;
            _emailService = emailService;
            _timeProvider = timeProvider;
        }

        public async Task<ConfirmationRequest> ConfirmationRequest(Guid requestId, string requestHash)
        {
            var confirmationRequest = await GetConfirmationRequest(requestId, requestHash);
            if (confirmationRequest.User.Active && confirmationRequest.Confirmed)
            {
                throw new UserEmailAlreadyConfirmedException();
            }

            var role = await _roleService.FindByName(UserConfirmationRole);
            if (role == null)
            {
                throw new RoleNotFoundException(UserConfirmationRole);
            }

            confirmationRequest.Confirmed = true;
            confirmationRequest.User.Role = role;
            await _confirmationRequestRepository.Update(confirmationRequest);
            return confirmationRequest;
        }

        public async Task SendEmailConfirmationRequest(User user, AdditionalSubject subject,
            ConfirmationRequestRevokeReason? revokeReason)
        {
            if (revokeReason == ConfirmationRequestRevokeReason.Resend)
            {
                await _confirmationRequestRepository.RevokeConfirmationRequest(user);
            }

            var confirmationRequest = new ConfirmationRequest
            {
                Id = Guid.NewGuid(),
                CreatedAt = _timeProvider.UtcNow(),
                ExpiredAt = _timeProvider.UtcNow().AddHours(10),
                User = user,
                AdditionalSubject = subject.ToString(),
                RequestType = RequestType.Email.ToString(),
                Subject = user.Email,
            };
            await _confirmationRequestRepository.Insert(confirmationRequest).ConfigureAwait(false);

            var confirmationHash = confirmationRequest.GetRequestHashCode().ToString(CultureInfo.CurrentCulture);
            var nameValueCollection = System.Web.HttpUtility.ParseQueryString(string.Empty);
            nameValueCollection.Add("?" + RequestIdParam, confirmationRequest.Id.ToString());
            nameValueCollection.Add(HashParam, confirmationHash);
            var uriEmailConfirmationLink = new Uri(_emailConfiguration.EmailConfirmationFilePath + nameValueCollection);
            var emailData = new EmailData
            {
                UriLink = uriEmailConfirmationLink,
                Sender = new EmailAddress(_sendGridConfiguration.SenderEmail, _sendGridConfiguration.SenderName),
                Recipients = new List<EmailAddress> { new (confirmationRequest.User.Email, confirmationRequest.User.FirstName) },
                EmailTemplate = EmailTemplate.ConfirmationEmail,
                AdditionalSubject = subject,
            };

            await _emailService.SendEmail(emailData);
        }

        private async Task<ConfirmationRequest> GetConfirmationRequest(Guid requestId, string requestHash)
        {
            var confirmationRequest = await _confirmationRequestRepository.FindByConfirmationRequestId(requestId);
            var hashConfirmFromDb = confirmationRequest.GetRequestHashCode().ToString(CultureInfo.CurrentCulture);

            if (confirmationRequest == null || !requestHash.Equals(hashConfirmFromDb, StringComparison.CurrentCulture))
            {
                throw new ConfirmationRequestNotFoundException();
            }

            if (confirmationRequest.ExpiredAt < _timeProvider.UtcNow())
            {
                throw new ConfirmationRequestExpiredException();
            }

            return confirmationRequest;
        }
    }
}
