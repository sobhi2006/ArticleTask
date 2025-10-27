namespace ArticleTask.DTOs.Responses.Auth;

public class AuthResponse
{
    public string? FullName { get; set; }
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime Expired { get; set; }
}