using FluentValidation;

namespace Application.Accounts.Commands
{
    public class RequestResetPasswordValidator : AbstractValidator<RequestResetPasswordCommand>
    {
        public RequestResetPasswordValidator()
        {
            RuleFor(r => r.Email)
               .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
               .WithMessage("Invalid email address.")
               .When(w => !string.IsNullOrEmpty(w.Email))
               .NotEmpty()
               .WithMessage("Email is required.");
        }
    }
}
