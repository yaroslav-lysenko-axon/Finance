using System.Linq;
using Authorization.Application.Models.Commands.TokenCommands;
using FluentValidation;

namespace Authorization.Application.Validators
{
    public class IssueTokenValidator : AbstractValidator<IssueTokenCommand>
    {
        public IssueTokenValidator()
        {
            RuleFor(user => user.Email).NotEmpty().EmailAddress();
            RuleFor(user => user.Password).NotEmpty().MinimumLength(8).Must(password => password.All(c => c != ' '));
        }
    }
}
