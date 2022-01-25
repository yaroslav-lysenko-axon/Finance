using System.Threading.Tasks;
using Authorization.Application.Extensions.Abstraction;
using Authorization.Application.Models.Commands.RegistrationCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly ICookiesExtensions _cookiesExtensions;
        private readonly IMediator _mediator;
        public RegistrationController(
            ICookiesExtensions cookiesExtensions,
            IMediator mediator)
        {
            _cookiesExtensions = cookiesExtensions;
            _mediator = mediator;
        }

        /// <summary>
        /// User registration.
        /// </summary>
        /// <param name="command">User information for registration.</param>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand command)
        {
            var response = await _mediator.Send(command);
            _cookiesExtensions.SetTokenCookie(response.RefreshTokenResponse.RefreshToken);
            return Ok(response.AccessTokenResponse);
        }
    }
}
