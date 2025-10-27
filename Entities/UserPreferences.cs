using ArticleTask.Enums;

namespace ArticleTask.Entities;

public class UserPreference
{
    public Guid Id { get; set; }
    public UserLanguage Language { get; set; }
    public UserTheme Theme { get; set; }
    public User User { get; set; } = null!;
    public Guid UserId{ get; set; } 
}