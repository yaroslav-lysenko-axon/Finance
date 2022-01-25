using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Application.Models.Commands.ConfirmationCommands
{
    public class ConfirmResendCommand : IRequest<OkResult>
    {
        public string Email { get; set; }
    }
}
