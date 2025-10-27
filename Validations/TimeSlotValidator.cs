using ArticleTask.Entities.WorkingHoursManagement;
using FluentValidation;

namespace ArticleTask.Validations;

public class TimeSlotValidator : AbstractValidator<TimeSlot>
{
    public TimeSlotValidator()
    {
        RuleFor(w => w.StartAt)
                .NotNull()
                .WithMessage("StartAt is required.");

        RuleFor(w => w.EndAt)
                .GreaterThan(w => w.StartAt)
                .WithMessage("EndAt time must be greater than StartAt");
    }
}