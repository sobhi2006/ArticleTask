using ArticleTask.DTOs.Requests.WorkingHours;
using ArticleTask.Entities.WorkingHoursManagement;
using FluentValidation;

namespace ArticleTask.Validations;

public class WorkingHoursValidator : AbstractValidator<WorkingHoursRequest>
{
    public WorkingHoursValidator()
    {
        RuleFor(w => w.Day)
                .IsInEnum();
        
        RuleFor(w => w.TimeSlot)
                .NotNull()
                .WithMessage("TimeSlot is required.")
                .Must(v => !v.GroupBy(st => new { st.StartAt, st.EndAt }).Any(g => g.Count() > 1))
                .WithMessage("duplicate time slot, it is not allowed")
                .Must(OverlappingSlots)
                .WithMessage("Overlapping between time slots, it is not allowed");

        RuleForEach(w => w.TimeSlot)
                .NotNull()
                .WithMessage("TimeSlot is required.")
                .SetValidator(new TimeSlotValidator());
    }
    bool OverlappingSlots(List<TimeSlot> timeSlots)
    {
        var Order = timeSlots.OrderBy(ts => ts.StartAt).ToList();
        return !Order.Zip(Order.Skip(1), (current, next) => new { current, next })
                     .Any(pair => pair.current.EndAt > pair.next.StartAt);
    }
}