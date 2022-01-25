using Authorization.Application.Models.Responses;
using MediatR;

namespace Authorization.Application.Models.Commands.TokenCommands
{
    public class RefreshTokenCommand : IRequest<TokenResponse>
    {
        public string RefreshToken { get; set; }
    }
}
