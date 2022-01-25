using System.Threading;
using System.Threading.Tasks;
using Authorization.Application.Models.Commands.ConfirmationCommands;
using Authorization.Domain.Repositories;
using Authorization.Domain.Services.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Application.Handlers.ConfirmationHandlerFolder
{
    public class ConfirmRegistrationHandler : IRequestHandler<ConfirmRegistrationCommand, OkResult>
    {
        private readonly IConfirmationRequestService _confirmationRequestService;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmRegistrationHandler(
            IConfirmationRequestService confirmationRequestService,
            IUnitOfWork unitOfWork)
        {
            _confirmationRequestService = confirmationRequestService;
            _unitOfWork = unitOfWork;
        }

        public async Task<OkResult> Handle(ConfirmRegistrationCommand request, CancellationToken cancellationToken)
        {
            await _confirmationRequestService.ConfirmationRequest(request.Id, request.Hash);
            await _unitOfWork.Commit();
            return new OkResult();
        }
    }
}
