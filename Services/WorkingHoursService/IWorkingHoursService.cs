using ArticleTask.DTOs.Requests.WorkingHours;
using ArticleTask.DTOs.Responses.WorkingHours;

namespace ArticleTask.Services.WorkingHoursService;

public interface IWorkingHoursService
{
    public Task<bool> AddNewWorkingHours(WorkingHoursRequest request, Guid UserId);
    public Task DeleteWorkingHours(Guid Id);
    public Task DeleteWorkingHours(DayOfWeek day, Guid UserId);
    public Task DeleteWorkingHours(WorkingHoursRequest request, Guid UserId);
    public Task<IEnumerable<WorkingHoursResponse>?> GetWorkingHoursByUserId(Guid Id);
    public Task<IEnumerable<WorkingHoursResponse>?> GetWorkingHours(DayOfWeek day, Guid UserId);
    public Task UpdateWorkingHours(Guid UserId, WorkingHoursRequest request);
    public Task<byte[]> GetScheduleUserReports(Guid UserId);
}