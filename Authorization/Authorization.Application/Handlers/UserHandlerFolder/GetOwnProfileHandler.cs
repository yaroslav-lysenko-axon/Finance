using System.Threading;
using System.Threading.Tasks;
using Authorization.Application.DTO;
using Authorization.Application.Models.Commands.UserCommands;
using Authorization.Domain.Services.Abstraction;
using AutoMapper;
using MediatR;

namespace Authorization.Application.Handlers.UserHandlerFolder
{
    public class GetOwnProfileHandler : IRequestHandler<GetUserCommand, UserDto>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public GetOwnProfileHandler(
            IUserService userService,
            IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserById(request.UserId);
            var response = _mapper.Map<UserDto>(user);
            return response;
        }
    }
}
