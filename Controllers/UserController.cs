using System.Security.Claims;
using ArticleTask.DTOs.Requests.Users;
using ArticleTask.DTOs.Responses.Users;
using ArticleTask.ExceptionHandlers;
using ArticleTask.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ArticleTask.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[Controller]")]
[ApiVersion("1.0")]
[Tags("Users")]

public class UserController(IUserService _userService) : ControllerBase
{
    private Guid _currentUserId
        => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<UserResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<UserResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("AddNewUserAsyncV1")]
    [EndpointSummary("Creates a new User")]
    [EndpointDescription("Creates a new User and returns the created result.")]
    public async Task<ActionResult<UserResponse>> AddNewUserAsync(UserRequest request)
    {
        System.Console.WriteLine("from add new user in controller");
        await _userService.AddNewUser(request, _currentUserId);
        return Created();
    }

    [HttpPut("{UserId:guid}")]
    [Authorize]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<UserResponse>>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<UserResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UpdateUserAsyncV1")]
    [EndpointSummary("Update a User")]
    [EndpointDescription("Update a User and returns the updated result.")]
    public async Task<ActionResult<UserResponse>> UpdateUserAsync(Guid UserId, UserRequest request)
    {
        await _userService.UpdateUser(UserId, request);
        return NoContent();
    }

    [HttpDelete("{Id:guid}")]
    [Authorize(Roles = "Admin")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<UserResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("DeleteUserAsyncV1")]
    [EndpointSummary("Delete a User by id")]
    [EndpointDescription("Delete a User and returns the deleted result.")]
    public async Task<IActionResult> DeleteUserAsync(Guid Id)
    {
        if (_currentUserId == Id)
            return BadRequest("you can't delete your-self");

        await _userService.DeleteUser(Id);
        return Ok("User Deleted Successfully");
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<UserResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetUserAsyncV1")]
    [EndpointSummary("Retrieves a User by id")]
    [EndpointDescription("Retrieves a User and returns the retrieve result.")]
    public async Task<ActionResult<UserResponse>> GetUserAsync(Guid Id)
    {
        var UserResponse = await _userService.GetUserById(Id);
        return Ok(UserResponse);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<UserResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetUsersAsync")]
    [EndpointSummary("Retrieve users by pagination")]
    [EndpointDescription("Retrieve users by pagination byDefault retrieve first page and contains 10 users.")]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsersAsync([FromQuery] int Page = 1, int PageSize = 10)
    {
        if (!(int.TryParse(Page.ToString(), out _) && int.TryParse(PageSize.ToString(), out _)))
            throw new BusinessException("Page and PageSize must be int", StatusCodes.Status400BadRequest);

        Page = Math.Max(1, Page);
        PageSize = Math.Clamp(PageSize, 1, 100);
        var UserResponse = await _userService.GetUsers(Page, PageSize);
        return Ok(UserResponse);
    }

    [HttpGet("pref/{id:guid}")]
    [Authorize]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<UserResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetUserPrefAsyncV1")]
    [EndpointSummary("Retrieves a User Pref by id")]
    [EndpointDescription("Retrieves a User Pref and returns the retrieve result.")]
    public async Task<ActionResult<UserResponse>> GetUserPrefAsync(Guid Id)
    {
        var UserPref = await _userService.GetUserPref(Id);
        return Ok(new{UserPref.Language, UserPref.Theme});
    }
}