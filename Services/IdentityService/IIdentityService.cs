using ArticleTask.DTOs.Responses.Auth;
using ArticleTask.Entities;

namespace ArticleTask.Services.IdentityService;

public interface IIdentityService
{
    public AuthResponse GenerateToken(User User);
    public string GenerateRefreshToken();
    public Guid GetClaimUserIdFromToken(string token);
    public Task<bool> SetRefreshAndExpired(string? Refresh, DateTime Expired, Guid UserId);
}