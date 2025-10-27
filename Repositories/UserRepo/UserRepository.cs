using ArticleTask.Data;
using ArticleTask.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArticleTask.Repositories.UserRepo;

public class UserRepository(AppDbContext context) : IUserRepository
{
    readonly AppDbContext _context = context;
    public async Task<bool> AddNewUser(User User)
    {
        await _context.Users.AddAsync(User);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task DeleteUser(Guid Id)
    {
        await _context.Users.Where(u => u.Id == Id).ExecuteDeleteAsync();
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistUser(Guid UserId)
    {
        return await _context.Users.AnyAsync(u => u.Id == UserId);
    }

    public async Task<bool> ExistUser(string Email)
    {
        return await _context.Users.AnyAsync(u => u.Email == Email);
    }

    public async Task<User?> GetUserById(Guid Id)
    {
        var User = await _context.Users.AsNoTracking()
                                             .Where(u => u.Id == Id)
                                             .FirstOrDefaultAsync();
        return User;
    }

    public async Task<User?> GetUserByEmail(string Email)
    {
        var User = await _context.Users.AsNoTracking()
                                  .Where(u => u.Email == Email).FirstOrDefaultAsync();

        return User;
    }

    public async Task<IEnumerable<User>?> GetUsers(int Page, int SizePage)
    {
        var Users = await _context.Users.AsNoTracking()
                                  .Skip((Page - 1) * SizePage)
                                  .Take(SizePage)
                                  .ToListAsync();
        return Users;
    }

    public async Task UpdateUser(User request)
    {
        _context.Users.Attach(request);
        _context.Entry(request).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AddNewUserPref(UserPreference request)
    {
        await _context.UserPreferences.AddAsync(request);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task UpdateUserPref(UserPreference request)
    {
        _context.UserPreferences.Attach(request);
        _context.Entry(request).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserPref(Guid UserId)
    {
        await _context.UserPreferences.Where(u => u.UserId == UserId).ExecuteDeleteAsync();
        await _context.SaveChangesAsync();
    }

    public async Task<UserPreference?> GetUserPref(Guid UserId)
    {
        return await _context.UserPreferences.Where(up => up.UserId == UserId).FirstOrDefaultAsync();
    }
}