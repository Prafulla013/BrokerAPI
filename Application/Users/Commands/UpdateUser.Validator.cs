using FluentValidation;
using System.Linq;

namespace Application.Users.Commands
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserValidator()
        {
            RuleFor(r => r.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .MaximumLength(300)
                .WithMessage("Maximum character limit is 300.");

            RuleFor(r => r.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .MaximumLength(300)
                .WithMessage("Maximum character limit is 300.");

            RuleFor(r => r.Email)
                .MaximumLength(100)
                .WithMessage("Maximum character limit is 100.")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
                .WithMessage("Invalid email address.")
                .When(w => !string.IsNullOrEmpty(w.Email))
                .NotEmpty()
                .WithMessage("Email is required.");

            RuleFor(r => r.PhoneNumber)
                .MaximumLength(10)
                .WithMessage("Maximum character limit is 10.")
                .Matches("^[0-9]+$")
                .WithMessage("Invalid phone number.")
                .When(w => !string.IsNullOrEmpty(w.PhoneNumber))
                .NotEmpty()
                .WithMessage("Phone number is required.");
        }
    }
}
