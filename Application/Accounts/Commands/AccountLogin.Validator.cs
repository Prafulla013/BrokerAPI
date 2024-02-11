using FluentValidation;

namespace Application.Accounts.Commands
{
    public class AccountLoginValidator : AbstractValidator<AccountLoginCommand>
    {
        public AccountLoginValidator()
        {
            RuleFor(r => r.Username)
               .NotEmpty()
               .WithMessage("Username is required.");

            RuleFor(r => r.Password)
                .NotEmpty()
                .WithMessage("Password is required.");
        }
    }
}
