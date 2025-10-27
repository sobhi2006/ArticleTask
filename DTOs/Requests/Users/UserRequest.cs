using ArticleTask.Enums;

namespace ArticleTask.DTOs.Requests.Users;

public class UserRequest
{
    public string? FName { get; set; }
    public string? LName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public DateTime DateOfBirth { get; set; }
    public UserRole Role { get; set; }
    public UserLanguage Language { get; set; }
    public UserTheme Theme { get; set; }
}