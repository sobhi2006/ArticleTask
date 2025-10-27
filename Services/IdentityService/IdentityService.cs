using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ArticleTask.DTOs.Responses.Auth;
using ArticleTask.Entities;
using ArticleTask.ExceptionHandlers;
using ArticleTask.Repositories.UserRepo;
using Microsoft.IdentityModel.Tokens;

namespace ArticleTask.Services.IdentityService;

public class JwtTokenService(IConfiguration configuration, IUserRepository userRepository) : IIdentityService
{
    private readonly IUserRepository _userRepository = userRepository;
    public AuthResponse GenerateToken(User User)
    {
        var JwtSettings = configuration.GetSection("JWT");

        var Claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, User!.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, User.Email!),
            new Claim(JwtRegisteredClaimNames.GivenName, User.FName!),
            new Claim(JwtRegisteredClaimNames.FamilyName, User.LName!),
            new Claim(ClaimTypes.Role, User.Role.ToString())
        };

        var expired = DateTime.UtcNow.AddMinutes(int.Parse(JwtSettings["ExpiredInMinutes"]!));

        var Descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(Claims),
            Expires = expired,
            Issuer = JwtSettings["Issuer"],
            Audience = JwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings["Key"]!))
                        , SecurityAlgorithms.HmacSha256)
        };

        var TokenHandler = new JwtSecurityTokenHandler();
        var Token = TokenHandler.CreateToken(Descriptor);

        return new AuthResponse
        {
            FullName = User.FName + " " + User.LName,
            Token = TokenHandler.WriteToken(Token),
            Expired = expired,
            RefreshToken = GenerateRefreshToken()
        };
    }

    public string GenerateRefreshToken()
    {
        var RandomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(RandomBytes);

        return Convert.ToBase64String(RandomBytes);
    }

    public Guid GetClaimUserIdFromToken(string token)
    {
        var TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidIssuer = configuration["JWT:Issuer"],
            ValidAudience = configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!.ToString())),
            ClockSkew = TimeSpan.Zero
        };

        var TokenHandler = new JwtSecurityTokenHandler();
        var principle = TokenHandler.ValidateToken(token, TokenValidationParameters, out _)
                    ?? throw new BusinessException("invalid token", StatusCodes.Status400BadRequest);

        var UserId = Guid.TryParse(principle.FindFirst(ClaimTypes.NameIdentifier)?.Value!, out Guid Id)
                                ? Id : throw new BusinessException("invalid token", StatusCodes.Status400BadRequest);
        return UserId;
    }

    public async Task<bool> SetRefreshAndExpired(string? Refresh, DateTime Expired, Guid UserId)
    {
        var User = await _userRepository.GetUserById(UserId) ??
                            throw new BusinessException("User not Found", StatusCodes.Status400BadRequest);
        User.RefreshToken = Refresh;
        User.Expired = Expired;
        await _userRepository.UpdateUser(User);
        return true;
    }
}