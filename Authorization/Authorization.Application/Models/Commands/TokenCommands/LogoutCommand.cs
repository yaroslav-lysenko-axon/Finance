using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Application.Models.Commands.TokenCommands
{
    public class LogoutCommand : IRequest<OkResult>
    {
        public string RefreshToken { get; set; }
    }
}
