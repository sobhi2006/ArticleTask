using System.Security.Claims;
using ArticleTask.DTOs.Requests.Auth;
using ArticleTask.DTOs.Responses.Users;
using ArticleTask.Services.IdentityService;
using ArticleTask.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ArticleTask.Controllers;


[ApiController]
[Route("api/v{version:apiVersion}/[Controller]")]
[ApiVersion("1.0")]
[Tags("Auth")]
public class AuthController(IIdentityService jwtTokenService, IUserService userService) : ControllerBase
{
    private Guid? _currentUserId
        => Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!, out Guid Id)? Id : null;

    [HttpPost("login")]
    [MapToApiVersion("1.0")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [Consumes("application/json")]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("LoginAsync")]
    [EndpointSummary("Login user by email and password.")]
    [EndpointDescription("Login user by email and password and retrieve result.")]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        var User = await userService.GetUserByEmailPassword(request);
        var token = jwtTokenService.GenerateToken(User);
        User.RefreshToken = token.RefreshToken;
        User.Expired = DateTime.UtcNow.AddHours(12);
        await userService.UpdateUser(User);
        return Ok(new { t = token, Id = HttpContext.User.Identities });
    }

    [HttpPost("Refresh")]
    [MapToApiVersion("1.0")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [Consumes("application/json")]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("RefreshAsync")]
    [EndpointSummary("Refresh token for user")]
    [EndpointDescription("Refresh token, and returns refreshed result.")]
    public async Task<IActionResult> RefreshAsync(RefreshTokenRequest request)
    {
        var UserId = jwtTokenService.GetClaimUserIdFromToken(request.Token!);
        var User = await userService.GetUser(UserId);

        if (User.RefreshToken != request.RefreshToken || User.Expired < DateTime.UtcNow)
            return Unauthorized("Invalid or expired refresh token, please login");

        var token = jwtTokenService.GenerateToken(User);
        await jwtTokenService.SetRefreshAndExpired(token.RefreshToken, DateTime.UtcNow.AddHours(12), UserId);

        return Ok(token);
    }

    [HttpPost("logout")]
    [Authorize]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("LogoutAsync")]
    [EndpointSummary("Logout user from the system")]
    [EndpointDescription("Logout user from the system, which token not expired.")]
    public async Task<IActionResult> LogoutAsync()
    {
        await jwtTokenService.SetRefreshAndExpired(null, DateTime.MinValue, (Guid)_currentUserId!);
        return Ok("Logout successfully");
    }

    [HttpGet("me")]
    [Authorize]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UserInfoAsync")]
    [EndpointSummary("Retrieve User Info for current user in the system")]
    [EndpointDescription("Retrieve User Info for current user in the system, and returns info result.")]
    public async Task<ActionResult<UserResponse>> UserInfoAsync()
    {
        var User = await userService.GetUserById((Guid)_currentUserId!);
        return Ok(User);
    }
}