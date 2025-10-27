using ArticleTask.Entities;

namespace ArticleTask.Repositories.UserRepo;

public interface IUserRepository
{
    public Task<bool> AddNewUser(User request);
    public Task<bool> ExistUser(Guid UserId);
    public Task<bool> ExistUser(string Email);
    public Task UpdateUser(User request);
    public Task DeleteUser(Guid Id);
    public Task<User?> GetUserById(Guid Id);
    public Task<UserPreference?> GetUserPref(Guid UserId);
    public Task<User?> GetUserByEmail(string Email);
    public Task<IEnumerable<User>?> GetUsers(int Page, int SizePage);

    public Task<bool> AddNewUserPref(UserPreference request);
    public Task UpdateUserPref(UserPreference request);
    public Task DeleteUserPref(Guid UserId);
}