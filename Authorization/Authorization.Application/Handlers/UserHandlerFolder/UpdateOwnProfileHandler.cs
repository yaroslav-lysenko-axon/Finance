using System.Threading;
using System.Threading.Tasks;
using Authorization.Application.Models.Commands.UserCommands;
using Authorization.Domain.Repositories;
using Authorization.Domain.Services.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Application.Handlers.UserHandlerFolder
{
    public class UpdateOwnProfileHandler : IRequestHandler<UpdateUserCommand, OkResult>
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOwnProfileHandler(
            IUserService userService,
            IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<OkResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var ownUser = await _userService.GetUserById(request.UserId);
            ownUser.FirstName = request.FirstName;
            ownUser.LastName = request.LastName;
            await _userService.UpdateUserProfile(ownUser);
            await _unitOfWork.Commit();
            return new OkResult();
        }
    }
}
