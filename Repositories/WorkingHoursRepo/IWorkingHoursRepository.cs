using ArticleTask.Entities.WorkingHoursManagement;

namespace ArticleTask.Repositories.WorkingHoursRepo;

public interface IWorkingHoursRepository
{
    public Task<bool> AddNewWorkingHours(UserWorkingHours request);
    public Task<bool> AddNewWorkingHoursAtDay(IEnumerable<WorkingHoursAtDay> request);
    public Task DeleteWorkingHours(Guid Id);
    public Task DeleteWorkingHours(DayOfWeek day, Guid UserId);
    public Task DeleteWorkingHours(DayOfWeek day, Guid UserId, List<TimeSlot> timeSlots);
    public Task<IEnumerable<UserWorkingHours>>? GetWorkingHoursByUserId(Guid UserId);
    public Task<IEnumerable<UserWorkingHours>>? GetWorkingHoursByDay(DayOfWeek day, Guid UserId);
    public Task<IEnumerable<UserWorkingHours>>? GetWorkingHours(int Page, int SizePage, Guid UserId);
    public Task<bool> IsOverLapping(TimeOnly StartAt, TimeOnly EndAt, DayOfWeek day, Guid UserId);
    public Task<IEnumerable<TimeSlot>>? GetTimeSlotByUserId(Guid UserId);
    public Task<Guid> DayIsExistsForUser(DayOfWeek day, Guid UserId);
}