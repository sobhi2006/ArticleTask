using ArticleTask.Entities.WorkingHoursManagement;

namespace ArticleTask.DTOs.Responses.WorkingHours;

public class WorkingHoursResponse
{
    public Guid UserId { get; set; }
    public DayOfWeek Day { get; set; }
    public List<TimeSlot> TimeSlot { get; set; } = [];

    private WorkingHoursResponse() { }

    public static WorkingHoursResponse FromModule(UserWorkingHours userWorkingHours)
    {
        WorkingHoursResponse response = new()
        {
            UserId = userWorkingHours.UserId,
            Day = userWorkingHours.Day,
            TimeSlot = userWorkingHours.WorkingHoursInDay.Select(whd => whd.TimeSlot).ToList()
        };
        return response;
    }
}