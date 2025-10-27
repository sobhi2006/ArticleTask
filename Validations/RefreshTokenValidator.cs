using ArticleTask.DTOs.Requests.Auth;
using FluentValidation;

namespace ArticleTask.Validations;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(u => u.Token).NotNull()
                .WithMessage("Token can't be null")
                .NotEmpty()
                .WithMessage("Token can't be empty");

        RuleFor(p => p.RefreshToken)
                .NotNull()
                .WithMessage("RefreshToken is required.")
                .NotEmpty()
                .WithMessage("RefreshToken can't be empty.");
    }
}