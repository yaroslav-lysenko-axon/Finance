using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Application.Models.Commands.ConfirmationCommands
{
    public class ConfirmRegistrationCommand : IRequest<OkResult>
    {
        public Guid Id { get; set; }
        public string Hash { get; set; }
    }
}
