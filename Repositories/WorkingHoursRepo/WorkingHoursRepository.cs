using System.Transactions;
using ArticleTask.Data;
using ArticleTask.Entities.WorkingHoursManagement;
using Microsoft.EntityFrameworkCore;

namespace ArticleTask.Repositories.WorkingHoursRepo;

public class WorkingHoursRepository(AppDbContext context) : IWorkingHoursRepository
{
    private readonly AppDbContext _context = context;
    public async Task<bool> AddNewWorkingHours(UserWorkingHours request)
    {
        await _context.UserWorkingHours.AddAsync(request);
        await _context.SaveChangesAsync();
        return true;
    }

    
    public async Task<bool> AddNewWorkingHoursAtDay(IEnumerable<WorkingHoursAtDay> request)
    {
        await _context.WorkingHoursAtDays.AddRangeAsync(request);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Guid> DayIsExistsForUser(DayOfWeek day, Guid UserId)
    {
        return await _context.UserWorkingHours.Where(wh => wh.Day == day && wh.UserId == UserId)
                                        .Select(wh => wh.Id)
                                        .FirstOrDefaultAsync();
    }

    public async Task DeleteWorkingHours(Guid Id)
    {
        await _context.UserWorkingHours.Where(wh => wh.UserId == Id).ExecuteDeleteAsync();
        await _context.SaveChangesAsync();
    }

    public async Task DeleteWorkingHours(DayOfWeek day, Guid UserId)
    {
        await _context.UserWorkingHours.Where(wh => wh.Day == day && wh.UserId == UserId).ExecuteDeleteAsync();
        await _context.SaveChangesAsync();
    }

    public async Task DeleteWorkingHours(DayOfWeek day, Guid UserId, List<TimeSlot> timeSlots)
    {
        var WorkingHoursId = await _context.UserWorkingHours.Where(wh => wh.Day == day
                                                                    && wh.UserId == UserId)
                                                            .Select(wh => wh.Id)
                                                            .FirstOrDefaultAsync();

        var WorkingHoursAtDay = await _context.WorkingHoursAtDays.Include(whd => whd.TimeSlot)
                                        .Where(whd => whd.WorkingHoursId == WorkingHoursId).ToListAsync();

        var matched = WorkingHoursAtDay.Where(whd => timeSlots.Any(ts => ts.StartAt == whd.TimeSlot.StartAt &&
                                                                    ts.EndAt == whd.TimeSlot.EndAt)).ToList();

        _context.WorkingHoursAtDays.RemoveRange(matched);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserWorkingHours>>? GetWorkingHours(int Page, int SizePage, Guid UserId)
    {
        var WorkingHours = await _context.UserWorkingHours.Include(wh => wh.WorkingHoursInDay)
                                                    .AsNoTracking()
                                                    .Where(wh => wh.UserId == UserId)
                                                    .Skip((Page - 1) * SizePage)
                                                    .Take(SizePage).ToListAsync();
        return WorkingHours;
    }

    public async Task<IEnumerable<UserWorkingHours>>? GetWorkingHoursByDay(DayOfWeek day, Guid UserId)
    {
        var WorkingHours = await _context.UserWorkingHours.Include(wh => wh.WorkingHoursInDay)
                                                    .AsNoTracking()
                                                    .Where(wh => wh.UserId == UserId && wh.Day == day)
                                                    .ToListAsync();
        return WorkingHours;
    }

    public async Task<IEnumerable<UserWorkingHours>>? GetWorkingHoursByUserId(Guid UserId)
    {
        var WorkingHours = await _context.UserWorkingHours.Include(wh => wh.WorkingHoursInDay)
                                                    .AsNoTracking()
                                                    .Where(wh => wh.UserId == UserId)
                                                    .ToListAsync();
        return WorkingHours;
    }

    public async Task<IEnumerable<TimeSlot>>? GetTimeSlotByUserId(Guid UserId)
    {
        var WorkingHoursId = await _context.UserWorkingHours.Where(wh => wh.UserId == UserId)
                                                            .Select(wh => wh.Id)
                                                            .FirstOrDefaultAsync();
        var ts = await _context.WorkingHoursAtDays.AsNoTracking()
                                                    .Where(whd => whd.WorkingHoursId == WorkingHoursId)
                                                    .Select(whd => whd.TimeSlot)
                                                    .ToListAsync();
        return ts;
    }

    public async Task<bool> IsOverLapping(TimeOnly StartAt, TimeOnly EndAt, DayOfWeek day, Guid UserId)
    {
        var WorkingHoursId = await _context.UserWorkingHours.Where(wh => wh.Day == day
                                                                        && wh.UserId == UserId)
                                                            .Select(wh => wh.Id)
                                                            .FirstOrDefaultAsync();

        return await _context.WorkingHoursAtDays.Where(whd => whd.WorkingHoursId == WorkingHoursId &&
                                                        ((whd.TimeSlot.EndAt > StartAt && whd.TimeSlot.EndAt < EndAt) ||
                                                         (whd.TimeSlot.StartAt > StartAt && whd.TimeSlot.StartAt < EndAt) ||
                                                         (whd.TimeSlot.StartAt == StartAt && whd.TimeSlot.EndAt == EndAt)
                                                        )).AnyAsync();
    }
}