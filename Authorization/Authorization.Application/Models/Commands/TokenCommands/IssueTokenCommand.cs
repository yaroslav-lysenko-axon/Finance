using System;
using Authorization.Application.Models.Responses;
using MediatR;

namespace Authorization.Application.Models.Commands.TokenCommands
{
    public class IssueTokenCommand : IRequest<TokenResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid ClientId { get; set; }
    }
}
