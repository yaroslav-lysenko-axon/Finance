using System;
using System.Linq;
using Authorization.Application.Models.Commands.RegistrationCommands;
using FluentValidation;

namespace Authorization.Application.Validators
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).Must(password => password.All(c => c != ' '));
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.ClientSecret).Must(x => x != Guid.Empty);
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
        }
    }
}
