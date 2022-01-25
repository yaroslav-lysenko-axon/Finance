using System;
using Authorization.Application.DTO;
using MediatR;

namespace Authorization.Application.Models.Commands.UserCommands
{
    public class GetUserCommand : IRequest<UserDto>
    {
    public Guid UserId { get; set; }
    }
}
