using System.Threading;
using System.Threading.Tasks;
using Authorization.Application.Models.Commands.ConfirmationCommands;
using Authorization.Domain.Enums;
using Authorization.Domain.Repositories;
using Authorization.Domain.Services.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Application.Handlers.ConfirmationHandlerFolder
{
    public class ConfirmResendHandler : IRequestHandler<ConfirmResendCommand, OkResult>
    {
        private readonly IConfirmationRequestService _confirmationRequestService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmResendHandler(
            IConfirmationRequestService confirmationRequestService,
            IUserService userService,
            IUnitOfWork unitOfWork)
        {
            _confirmationRequestService = confirmationRequestService;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<OkResult> Handle(ConfirmResendCommand request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetInactiveUser(request.Email);
            await _confirmationRequestService.SendEmailConfirmationRequest(user, AdditionalSubject.Registration, ConfirmationRequestRevokeReason.Resend);
            await _unitOfWork.Commit();
            return new OkResult();
        }
    }
}
