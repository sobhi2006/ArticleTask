using ArticleTask.DTOs.Requests.Auth;
using FluentValidation;

namespace ArticleTask.Validations;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
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

    }
}