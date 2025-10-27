namespace ArticleTask.DTOs.Requests.Auth;

public class RefreshTokenRequest
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}