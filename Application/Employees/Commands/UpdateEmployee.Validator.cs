using Common.Enumerations;
using FluentValidation;
using System;
using System.Linq;

namespace Application.Employees.Commands
{
    public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeValidator()
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

            RuleFor(r => r.Username)
                .MinimumLength(3)
                .WithMessage("Minimum character requirement is 3.")
                .MaximumLength(100)
                .WithMessage("Maximum character limit is 100.")
                .Matches(@"^[a-zA-Z][\w\s]*") // Username must start with alphabet and then can have alphanumeric character
                .WithMessage("Invalid userame. Must start with alphabet and can only have alphanumeric.")
                .When(w => !string.IsNullOrEmpty(w.Username))
                .NotEmpty()
                .WithMessage("Username is required.")
                .When(w => w.HasSystemAccess == true);

            RuleFor(r => r.Street)
                .NotEmpty()
                .WithMessage("Street is required.")
                .MaximumLength(300)
                .WithMessage("Maximum character limit is 300.");

            RuleFor(r => r.City)
               .NotEmpty()
               .WithMessage("City is required.")
               .MaximumLength(100)
               .WithMessage("Maximum character limit is 100.");

            RuleFor(r => r.State)
               .NotEmpty()
               .WithMessage("State is required.")
               .MaximumLength(100)
               .WithMessage("Maximum character limit is 300.");

            RuleFor(r => r.ZipCode)
               .NotEmpty()
               .WithMessage("Zip code is required.")
               .MaximumLength(10)
               .WithMessage("Maximum character limit is 10.");

            RuleFor(r => r.BrokerId)
                .GreaterThan(0)
                .WithMessage("Invalid user request.");

            RuleFor(r => r.Subdomain)
                .NotEmpty()
                .WithMessage("Invalid user request.");

            RuleFor(r => r.UserType)
                .Must(type =>
                {
                    var validTypes = new UserType[] { UserType.Employee };
                    return validTypes.Contains(type);
                })
                .WithMessage("Invalid user type.");
        }
    }
}
