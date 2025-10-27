using ArticleTask.DTOs.Requests.Users;
using FluentValidation;

namespace ArticleTask.Validations;

public class UserValidator : AbstractValidator<UserRequest>
{
    public UserValidator()
    {
        RuleFor(u => u.FName).NotNull()
                             .WithMessage("First Name can't be null")
                             .NotEmpty()
                             .WithMessage("First Name can't be empty");

        RuleFor(u => u.LName).NotNull()
                             .WithMessage("Last Name can't be null")
                             .NotEmpty()
                             .WithMessage("Last Name can't be empty");

        RuleFor(u => u.Email).NotNull()
                             .WithMessage("Email can't be null")
                             .NotEmpty()
                             .WithMessage("Email can't be empty")
                             .EmailAddress()
                             .WithMessage("Enter valid email");

        RuleFor(p => p.Password)
                .NotNull()
                .WithMessage("Password is required.")
                .NotEmpty()
                .WithMessage("Password can't be empty.")
                .MinimumLength(8)
                .WithMessage("Password can't be less than 8 character.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at less one capital letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at less one small letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at less one digit")
                .Matches(@"[!@#$%^&*()_+-=[\]{}<>?;':""]").WithMessage("Password must contain at less one punctuation");

        RuleFor(u => u.Role)
                .IsInEnum();

        RuleFor(u => u.Language)
                .IsInEnum();

        RuleFor(u => u.Theme)
                .IsInEnum();
                
        RuleFor(p => p.DateOfBirth)
                .NotNull()
                .WithMessage("DateOfBirth is required.")
                .Must(v =>DateTime.UtcNow.AddYears(-100) <= v && DateTime.UtcNow.AddYears(-18) >= v)
                .WithMessage("Age must be between (18 - 100).");
    }
}