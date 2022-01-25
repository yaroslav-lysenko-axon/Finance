using System;
using System.Net;
using System.Threading.Tasks;
using Authorization.Application.Models.Commands.ConfirmationCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfirmationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ConfirmationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Confirm registration.
        /// </summary>
        /// <remarks>
        /// User registration confirmation.
        /// </remarks>
        /// <param name="id">Confirmation request id.</param>
        /// <param name="hash">Hash code combined from confirmation id and user id.</param>
        [HttpPatch("{id}/registration")]
        public async Task<IActionResult> ConfirmRegistration([FromRoute] Guid id, [FromForm] string hash)
        {
            var request = new ConfirmRegistrationCommand
            {
                Id = id,
                Hash = hash,
            };
            await _mediator.Send(request);
            return StatusCode((int)HttpStatusCode.Accepted);
        }

        [HttpPost("password")]
        public async Task<IActionResult> SendPasswordRecoveryEmail([FromBody] SendPasswordRecoveryEmailCommand request)
        {
            await _mediator.Send(request);
            return StatusCode(204);
        }

        [HttpPost("resend")]
        public async Task<IActionResult> ResendConfirmation([FromBody] ConfirmResendCommand request)
        {
            await _mediator.Send(request);
            return StatusCode(204);
        }
    }
}
