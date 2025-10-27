using ArticleTask.DTOs.Requests.Auth;
using ArticleTask.DTOs.Requests.Users;
using ArticleTask.DTOs.Responses.Users;
using ArticleTask.Entities;
namespace ArticleTask.Services.UserService;
public interface IUserService
{
    public Task<bool> AddNewUser(UserRequest request, Guid UserId);
    public Task DeleteUser(Guid Id);
    public Task<UserResponse> GetUserById(Guid Id);
    public Task<User> GetUser(Guid Id);
    public Task<UserPreference> GetUserPref(Guid UserId);
    public Task<UserResponse> GetUserByEmail(string Email);
    public Task<IEnumerable<UserResponse>> GetUsers(int Page, int SizePage);
    public Task UpdateUser(Guid Id, UserRequest request);
    public Task UpdateUser(User request);
    public Task<User> GetUserByEmailPassword(LoginRequest request);
}