using ArticleTask.Entities;
using ArticleTask.Enums;

namespace ArticleTask.DTOs.Responses.Users;

public class UserResponse
{
    public Guid Id { get; set; }
    public string FName { get; set; } = null!;
    public string LName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public UserRole Role { get; set; }
    public Guid? CreatedBy { get; set; }
    private UserResponse() { }
    public static UserResponse FromModule(User user)
    {
        UserResponse response = new()
        {
            Id = user.Id,
            FName = user.FName,
            LName = user.LName,
            Email = user.Email,
            Password = user.Password,
            DateOfBirth = user.DateOfBirth,
            Role = user.Role,
            CreatedBy = user.CreatedBy
        };
        return response;
    }
}