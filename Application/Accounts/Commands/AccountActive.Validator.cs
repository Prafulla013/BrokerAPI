using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Accounts.Commands
{
    public class AccountActiveValidator : AbstractValidator<AccountActivateCommand>
    {
        public AccountActiveValidator()
        {
            RuleFor(r => r.Email)
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
                .WithMessage("Invalid email address.")
                .When(w => !string.IsNullOrEmpty(w.Email))
                .NotEmpty()
                .WithMessage("Email is required.");

            RuleFor(r => r.Token)
                .NotEmpty()
                .WithMessage("Token is required.");

            RuleFor(r => r.Password)
                .MinimumLength(8)
                .WithMessage("Minimum character requirement is 8.")
                .MaximumLength(100)
                .WithMessage("Maximum character limit is 100.")
                .Must(password =>
                {
                    var result = Regex.IsMatch(password, "[A-Z]+");
                    return result;
                })
                .WithMessage("Must contain at least one uppercase letter [A-Z].")
                .Must(password =>
                {
                    var result = Regex.IsMatch(password, "[a-z]+");
                    return result;
                })
                .WithMessage("Must contain at least one lowercase letter [a-z].")
                .Must(password =>
                {
                    var result = Regex.IsMatch(password, "[0-9]+");
                    return result;
                })
                .WithMessage("Must contain at least one number [0-9].")
                .Must(password =>
                {
                    var result = Regex.IsMatch(password, "[!@#$%^&*()_=+]+");
                    return result;
                })
                .WithMessage("Must contain at least one special charater [!@#$%^&*()_=+].")
                .When(w => !string.IsNullOrEmpty(w.Password))
                .NotEmpty()
                .WithMessage("Password is required.");
        }
    }
}
