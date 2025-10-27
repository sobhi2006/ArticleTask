using ArticleTask.DTOs.Requests.Articles;
using FluentValidation;

namespace ArticleTask.Validations;


public class ArticleValidator : AbstractValidator<ArticleRequest>
{
    public ArticleValidator()
    {
        RuleFor(a => a.Title).NotNull()
                             .WithMessage("Title can't be null")
                             .NotEmpty()
                             .WithMessage("Title can't be empty");

        RuleFor(a => a.Tags).NotNull()
                             .WithMessage("Tags can't be null")
                             .NotEmpty()
                             .WithMessage("Tags can't be empty");

        RuleFor(a => a.Content).NotNull()
                             .WithMessage("Content can't be null")
                             .NotEmpty()
                             .WithMessage("Content can't be empty");

        RuleFor(a => a.Image)
                .NotNull()
                .WithMessage("Enter image of article")
                .Must(i => i?.Length > 0 && i.ContentType.Length > 0)
                .WithMessage("image can't empty");

        RuleFor(a => a.Category)
                .IsInEnum();

        RuleFor(a => a.PublishStatus)
                .IsInEnum();
                
        RuleFor(p => p.PublishedAt)
                .NotNull()
                .WithMessage("PublishedAt is required.")
                .Must(v =>DateTime.UtcNow.AddSeconds(-5) < v)
                .WithMessage("PublishedAt must be at now or in future.");
    }
}