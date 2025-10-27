using ArticleTask.Entities.Common;

namespace ArticleTask.Entities.WorkingHoursManagement;

public class UserWorkingHours : CommonFields
{
    public DayOfWeek Day { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<WorkingHoursAtDay> WorkingHoursInDay { get; set; } = [];
}
