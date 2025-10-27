using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ArticleTask.DTOs.Requests.WorkingHours;
using ArticleTask.DTOs.Responses.WorkingHours;
using ArticleTask.Entities.WorkingHoursManagement;
using ArticleTask.ExceptionHandlers;
using ArticleTask.Repositories.WorkingHoursRepo;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Caching.Memory;

namespace ArticleTask.Services.WorkingHoursService;

public class WorkingHoursService(IWorkingHoursRepository workingHoursRepository, IMemoryCache cache) : IWorkingHoursService
{
    readonly IMemoryCache _cache = cache;
    string _KeyCache = "workingCache";
    List<string> _KeyCacheList = [];
    private readonly IWorkingHoursRepository _workingHoursRepository = workingHoursRepository;
    public async Task<bool> AddNewWorkingHours(WorkingHoursRequest request, Guid UserId)
    {
        foreach (var ts in request.TimeSlot)
        {
            if (await _workingHoursRepository.IsOverLapping(ts.StartAt, ts.EndAt, request.Day, UserId))
                throw new BusinessException("Conflict time slot, show your time slots and enter again.",
                                            StatusCodes.Status400BadRequest);
        }

        Guid WorkingHoursId = await _workingHoursRepository.DayIsExistsForUser(request.Day, UserId);
        if (WorkingHoursId == Guid.Empty)
        {
            Guid NewWorkingId = Guid.NewGuid();
            UserWorkingHours userWorking = new()
            {
                Id = NewWorkingId,
                UserId = UserId,
                CreatedBy = UserId,
                Day = request.Day,
                WorkingHoursInDay = request.TimeSlot.Select(ts => new WorkingHoursAtDay()
                {
                    Id = Guid.NewGuid(),
                    WorkingHoursId = NewWorkingId,
                    TimeSlot = ts
                }).ToList()
            };
            await _workingHoursRepository.AddNewWorkingHours(userWorking);
        }
        else
        {
            await _workingHoursRepository.AddNewWorkingHoursAtDay(request.TimeSlot.Select(ts => new WorkingHoursAtDay()
            {
                Id = Guid.NewGuid(),
                WorkingHoursId = WorkingHoursId,
                TimeSlot = ts
            }).ToList());
        }
            
        return true;
    }

    public async Task DeleteWorkingHours(Guid Id)
    {
        await _workingHoursRepository.DeleteWorkingHours(Id);
    }

    public async Task DeleteWorkingHours(DayOfWeek day, Guid UserId)
    {
        await _workingHoursRepository.DeleteWorkingHours(day, UserId);
    }

    public async Task DeleteWorkingHours(WorkingHoursRequest request, Guid UserId)
    {
        await _workingHoursRepository.DeleteWorkingHours(request.Day, UserId, request.TimeSlot.ToList());
    }

    public async Task<IEnumerable<WorkingHoursResponse>?> GetWorkingHours(DayOfWeek day, Guid UserId)
    {
        var WorkingHours = await _workingHoursRepository.GetWorkingHoursByDay(day, UserId)!;
        return WorkingHours.Select(WorkingHoursResponse.FromModule);
    }

    public async Task<IEnumerable<WorkingHoursResponse>?> GetWorkingHoursByUserId(Guid UserId)
    {
        var WorkingHours = await _workingHoursRepository.GetWorkingHoursByUserId(UserId)!;
        return WorkingHours.Select(WorkingHoursResponse.FromModule);
    }

    public async Task UpdateWorkingHours(Guid UserId, WorkingHoursRequest request)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await this.DeleteWorkingHours(request.Day, UserId);
        await this.AddNewWorkingHours(request, UserId);

        transaction.Complete();
    }
    
    
    public async Task<byte[]> GetScheduleUserReports(Guid UserId)
    {
        return await _cache.GetOrCreate(_KeyCacheList.FirstOrDefault(_KeyCache + UserId), async entry =>
        {
           
           System.Console.WriteLine("From Db :-)");
           entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
           entry.Size = 1;

           _KeyCacheList.Add(_KeyCache + UserId);
           var csvBuilder = new StringBuilder();
           csvBuilder.AppendLine("\t\t\t\tSummary Of Articles");
           csvBuilder.AppendLine();
           csvBuilder.AppendLine();

           var TimeSlots = await _workingHoursRepository.GetTimeSlotByUserId(UserId)! ??
               throw new BusinessException("Time Slot has not found", StatusCodes.Status400BadRequest);

           int RowNumber = 1;
           foreach (var ts in TimeSlots)
           {
               csvBuilder.AppendLine($"{RowNumber}-) {ts.StartAt}---{ts.EndAt}");
               csvBuilder.AppendLine();
               RowNumber++;
           }
           csvBuilder.AppendLine("\t\t\t\tTHE END");

           var fileBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
           return fileBytes;
       })!;
    }
}