using System.Threading;
using System.Threading.Tasks;
using Authorization.Application.Models.Commands.ConfirmationCommands;
using Authorization.Domain.Repositories;
using Authorization.Domain.Services.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Application.Handlers.ConfirmationHandlerFolder
{
    public class SendPasswordRecoveryEmailHandler : IRequestHandler<SendPasswordRecoveryEmailCommand, OkResult>
    {
        private readonly IPasswordService _passwordService;
        private readonly IUnitOfWork _unitOfWork;

        public SendPasswordRecoveryEmailHandler(
            IPasswordService passwordService,
            IUnitOfWork unitOfWork)
        {
            _passwordService = passwordService;
            _unitOfWork = unitOfWork;
        }

        public async Task<OkResult> Handle(SendPasswordRecoveryEmailCommand request, CancellationToken cancellationToken)
        {
            await _passwordService.SendPasswordRecoveryEmail(request.Email);
            await _unitOfWork.Commit();
            return new OkResult();
        }
    }
}
