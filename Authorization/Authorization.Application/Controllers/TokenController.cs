using System.Threading.Tasks;
using Authorization.Application.Extensions.Abstraction;
using Authorization.Application.Models.Commands.TokenCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ICookiesExtensions _cookieExtensions;
        private readonly IMediator _mediator;

        public TokenController(
            ICookiesExtensions cookieExtensions,
            IMediator mediator)
        {
            _cookieExtensions = cookieExtensions;
            _mediator = mediator;
        }

        [HttpPost("credentials")]
        public async Task<IActionResult> IssueToken([FromBody] IssueTokenCommand command)
        {
            var response = await _mediator.Send(command);
            _cookieExtensions.SetTokenCookie(response.RefreshTokenResponse.RefreshToken);
            return Ok(response.AccessTokenResponse);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshIssueToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var command = new RefreshTokenCommand()
            {
                RefreshToken = refreshToken,
            };

            var response = await _mediator.Send(command);
            _cookieExtensions.SetTokenCookie(response.RefreshTokenResponse.RefreshToken);
            return Ok(response.AccessTokenResponse);
        }

        [HttpDelete("me")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var command = new LogoutCommand
            {
                RefreshToken = refreshToken,
            };
            Response.Cookies.Delete("refreshToken");
            await _mediator.Send(command);
            return Ok();
        }
    }
}
