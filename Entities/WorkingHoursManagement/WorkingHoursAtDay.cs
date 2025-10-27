namespace ArticleTask.Entities.WorkingHoursManagement;

public class WorkingHoursAtDay
{
    public Guid Id { get; set; }
    public TimeSlot TimeSlot { get; set; } = null!;
    public UserWorkingHours WorkingHours = null!;
    public Guid WorkingHoursId { get; set; }
}