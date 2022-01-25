using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Application.Models.Commands.UserCommands
{
    public class ChangeUserPasswordCommand : IRequest<OkResult>
    {
        public Guid UserId { get; set; }
        [Required]
        [StringLength(90, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 8)]
        public string OldPassword { get; set; }
        [Required]
        [StringLength(90, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 8)]
        public string NewPassword { get; set; }
    }
}
