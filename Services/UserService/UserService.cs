using System.Text;
using System.Transactions;
using ArticleTask.DTOs.Requests.Auth;
using ArticleTask.DTOs.Requests.Users;
using ArticleTask.DTOs.Responses.Users;
using ArticleTask.Entities;
using ArticleTask.ExceptionHandlers;
using ArticleTask.Repositories.UserRepo;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;

namespace ArticleTask.Services.UserService;

public class UserService(IUserRepository userRepository, IDataProtectionProvider protectionProvider) : IUserService
{
    readonly IUserRepository _UserRepository = userRepository;
    private readonly IDataProtector _protector = protectionProvider.CreateProtector("User.Protection");
    public async Task<bool> AddNewUser(UserRequest request, Guid UserId)
    {
        if (await _UserRepository.ExistUser(request.Email!))
            throw new BusinessException("User already has found", StatusCodes.Status400BadRequest);

        try
        {
            User NewUser = new()
            {
                Id = Guid.NewGuid(),
                FName = request.FName!,
                LName = request.LName!,
                Email = request.Email!,
                CreatedBy = UserId,
                DateOfBirth = request.DateOfBirth,
                Password = _protector.Protect(request.Password!),
                Role = request.Role,
            };
            UserPreference userPreference = new()
            {
                Id = Guid.NewGuid(),
                UserId = NewUser.Id,
                Language = request.Language,
                Theme = request.Theme
            };
            await _UserRepository.AddNewUser(NewUser);
            await _UserRepository.AddNewUserPref(userPreference);
        }
        catch (Exception ex)
        {
            throw new BusinessException(ex.Message, StatusCodes.Status500InternalServerError);
        }
        return true;
    }

    public async Task DeleteUser(Guid Id)
    {
        if (!await _UserRepository.ExistUser(Id))
            throw new BusinessException("User has not found to delete", StatusCodes.Status400BadRequest);

        await _UserRepository.DeleteUser(Id);
    }

    public async Task<UserResponse> GetUserById(Guid Id)
    {
        var User = await _UserRepository.GetUserById(Id) ??
            throw new BusinessException("User has not found", StatusCodes.Status400BadRequest);

        return UserResponse.FromModule(User);
    }

    public async Task<UserResponse> GetUserByEmail(string Email)
    {
        var User = await _UserRepository.GetUserByEmail(Email) ??
            throw new BusinessException("Users has not found", StatusCodes.Status400BadRequest);

        return UserResponse.FromModule(User);
    }

    public async Task<IEnumerable<UserResponse>> GetUsers(int Page, int SizePage)
    {
        var Users = await _UserRepository.GetUsers(Page, SizePage) ??
            throw new BusinessException("Users has not found", StatusCodes.Status400BadRequest);

        return Users.Select(UserResponse.FromModule);
    }

    public async Task UpdateUser(Guid Id, UserRequest request)
    {
        if (await _UserRepository.ExistUser(request.Email!))
            throw new BusinessException("Conflict with another User", StatusCodes.Status409Conflict);

        var User = await _UserRepository.GetUserById(Id) ??
            throw new BusinessException("Users has not found", StatusCodes.Status400BadRequest);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await this.DeleteUser(Id);
        await this.AddNewUser(request, Id);

        transaction.Complete();
    }

    public async Task<User> GetUserByEmailPassword(LoginRequest request)
    {
        var User = await _UserRepository.GetUserByEmail(request.Email!);

        if (User is null || _protector.Unprotect(User.Password) != request.Password)
            throw new BusinessException("Email or Password is UnCorrect", StatusCodes.Status400BadRequest);

        return User;
    }

    public async Task UpdateUser(User request)
    {
        await _UserRepository.UpdateUser(request);
    }

    public async Task<User> GetUser(Guid Id)
    {
        var User = await _UserRepository.GetUserById(Id) ??
            throw new BusinessException("User has not found", StatusCodes.Status400BadRequest);

        return User;
    }

    public async Task<UserPreference> GetUserPref(Guid UserId)
    {
        var userPref = await _UserRepository.GetUserPref(UserId) ??
            throw new BusinessException("User has not found", StatusCodes.Status400BadRequest);
        return userPref;
    }
}