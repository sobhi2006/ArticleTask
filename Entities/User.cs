using ArticleTask.Entities.Common;
using ArticleTask.Entities.WorkingHoursManagement;
using ArticleTask.Enums;

namespace ArticleTask.Entities;

public class User : CommonFields
{
    public string FName { get; set; } = null!;
    public string LName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public UserRole Role { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime Expired { get; set; } = DateTime.MinValue;
    public ICollection<Article> Articles = [];
    public ICollection<UserWorkingHours> WorkingHours = [];
}