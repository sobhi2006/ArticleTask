using ArticleTask.Entities.WorkingHoursManagement;

namespace ArticleTask.DTOs.Requests.WorkingHours;

public class WorkingHoursRequest
{
    public DayOfWeek Day { get; set; }
    public List<TimeSlot> TimeSlot { get; set; } = [];
}