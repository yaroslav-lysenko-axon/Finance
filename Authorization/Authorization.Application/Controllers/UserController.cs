using System.Net;
using System.Threading.Tasks;
using Authorization.Application.Extensions;
using Authorization.Application.Models.Commands.UserCommands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get own profile.
        /// </summary>
        /// <remarks>
        /// Gets its own profile by identifier from jwt`s claims.
        /// </remarks>
        /// <response code="200">Getting was successful.</response>
        /// <response code="401">The user is not authorized.</response>
        /// <response code="403">Getting is forbidden.</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [HttpGet("me")]
        [Authorize("scope-get-own-profile")]
        public async Task<IActionResult> GetOwnProfile()
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetAuthorizedUserId();

            var request = new GetUserCommand
            {
                UserId = userId.GetValueOrDefault(),
            };

            var response = await _mediator.Send(request);
            return Ok(response);
        }

        /// <summary>
        /// Update own profile.
        /// </summary>
        /// <remarks>
        /// Updates its own profile by identifier from jwt`s claims.
        /// </remarks>
        /// <param name="userDto">User information to update the user profile.</param>
        /// <response code="202">The update has been accepted.</response>
        [ProducesResponseType(202)]
        [HttpPost("me")]
        [Authorize("scope-update-own-profile")]
        public async Task<IActionResult> UpdateOwnProfile([FromBody]UpdateUserCommand userDto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetAuthorizedUserId();

            var request = new UpdateUserCommand
            {
                UserId = userId.GetValueOrDefault(),
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
            };

            await _mediator.Send(request);
            return StatusCode((int)HttpStatusCode.Accepted);
        }

        /// <summary>
        /// Change own password.
        /// </summary>
        /// <remarks>
        /// Changes its own password by identifier from statements jwt.
        /// </remarks>
        /// <param name="password">New user password.</param>
        /// <response code="202">The update has been accepted.</response>
        [ProducesResponseType(202)]
        [HttpPatch("me/password")]
        [Authorize("scope-get-own-profile")]
        public async Task<IActionResult> ChangeOwnProfile([FromBody]ChangeUserPasswordCommand password)
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetAuthorizedUserId();

            var request = new ChangeUserPasswordCommand
            {
                UserId = userId.GetValueOrDefault(),
                NewPassword = password.NewPassword,
                OldPassword = password.OldPassword,
            };

            await _mediator.Send(request);
            return StatusCode((int)HttpStatusCode.Accepted);
        }
    }
}
