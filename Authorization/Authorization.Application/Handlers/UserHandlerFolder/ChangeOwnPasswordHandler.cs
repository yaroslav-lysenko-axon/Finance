using System.Threading;
using System.Threading.Tasks;
using Authorization.Application.Models.Commands;
using Authorization.Application.Models.Commands.UserCommands;
using Authorization.Domain.Repositories;
using Authorization.Domain.Services.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Application.Handlers.UserHandlerFolder
{
    public class ChangeOwnPasswordHandler : IRequestHandler<ChangeUserPasswordCommand, OkResult>
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeOwnPasswordHandler(
            IUserService userService,
            IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<OkResult> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var userSecurityInfo = await _userService.ChangeUserPassword(request.UserId, request.OldPassword, request.NewPassword);
            var ownUser = await _userService.GetUserById(request.UserId);
            ownUser.PasswordHash = userSecurityInfo.PasswordHash;
            ownUser.Salt = userSecurityInfo.Salt;
            await _userService.UpdateUserProfile(ownUser);
            await _unitOfWork.Commit();
            return new OkResult();
        }
    }
}
