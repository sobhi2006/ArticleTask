using System.Security.Claims;
using System.Threading.Tasks;
using ArticleTask.DTOs.Requests.WorkingHours;
using ArticleTask.DTOs.Responses.WorkingHours;
using ArticleTask.Services.WorkingHoursService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ArticleTask.Controllers;


[ApiController]
[Route("api/v{version:apiVersion}/[Controller]")]
[ApiVersion("1.0")]
[Tags("WorkingHours")]
public class WorkingHoursController(IWorkingHoursService _workingHoursService) : ControllerBase
{
    private Guid _currentUserId
        => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    [HttpPost]
    [Authorize]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<WorkingHoursResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<WorkingHoursResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("AddNewWorkingHoursAsyncV1")]
    [EndpointSummary("Creates a new WorkingHours")]
    [EndpointDescription("Creates a new WorkingHours and returns the created result.")]
    public async Task<ActionResult<WorkingHoursResponse>> AddNewWorkingHoursAsync(WorkingHoursRequest request)
    {
        await _workingHoursService.AddNewWorkingHours(request, _currentUserId);
        return Created();
    }

    [HttpPut]
    [Authorize]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<WorkingHoursResponse>>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<WorkingHoursResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UpdateWorkingHoursAsyncV1")]
    [EndpointSummary("Update a WorkingHours")]
    [EndpointDescription("Update a WorkingHours and returns the updated result.")]
    public async Task<ActionResult<WorkingHoursResponse>> UpdateWorkingHoursAsync(WorkingHoursRequest request)
    {
        await _workingHoursService.UpdateWorkingHours(_currentUserId, request);
        return NoContent();
    }

    [HttpDelete]
    [Authorize]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<WorkingHoursResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<WorkingHoursResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("DeleteWorkingHoursByIdAsyncV1")]
    [EndpointSummary("Delete a WorkingHours by id")]
    [EndpointDescription("Delete a WorkingHours and returns the deleted result.")]
    public async Task<ActionResult> DeleteWorkingHoursByIdAsync()
    {
        await _workingHoursService.DeleteWorkingHours(_currentUserId);
        return Ok("WorkingHours deleted successfully");
    }

    [HttpDelete("day-user/{day}")]
    [Authorize]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<WorkingHoursResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<WorkingHoursResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("DeleteWorkingHoursByDayAsyncV1")]
    [EndpointSummary("Delete a WorkingHours by day and user id")]
    [EndpointDescription("Delete a WorkingHours day and user id and returns the deleted result.")]
    public async Task<ActionResult> DeleteWorkingHoursByDayAsync(DayOfWeek day)
    {
        await _workingHoursService.DeleteWorkingHours(day, _currentUserId);
        return Ok("WorkingHours deleted successfully");
    }

    [HttpDelete("time-slot-day")]
    [Authorize]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<WorkingHoursResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<WorkingHoursResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("DeleteWorkingHoursAsyncV1")]
    [EndpointSummary("Delete a WorkingHours by day and user id")]
    [EndpointDescription("Delete a WorkingHours day and user id and returns the deleted result.")]
    public async Task<ActionResult> DeleteWorkingHoursAsync(WorkingHoursRequest request)
    {
        await _workingHoursService.DeleteWorkingHours(request, _currentUserId);
        return Ok("WorkingHours deleted successfully");
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [Authorize]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<WorkingHoursResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetWorkingHoursByUserIdAsyncV1")]
    [EndpointSummary("Retrieves WorkingHours by UserId")]
    [EndpointDescription("Retrieves WorkingHours by UserId and returns the retrieve result.")]
    public async Task<ActionResult<List<WorkingHoursResponse>>> GetWorkingHoursByUserIdAsync()
    {
        var WorkingHours = await _workingHoursService.GetWorkingHoursByUserId(_currentUserId);
        return Ok(WorkingHours);
    }

    [HttpGet("day/{day}")]
    [MapToApiVersion("1.0")]
    [Authorize]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<WorkingHoursResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetWorkingHoursByDayAsyncV1")]
    [EndpointSummary("Retrieves WorkingHours ByDay")]
    [EndpointDescription("Retrieves WorkingHours ByDay and returns the retrieve result.")]
    public async Task<ActionResult<List<WorkingHoursResponse>>> GetWorkingHoursByDayAsync(DayOfWeek day)
    {
        var WorkingHours = await _workingHoursService.GetWorkingHours(day, _currentUserId);
        return Ok(WorkingHours);
    }

    [HttpGet("Reports/{UserId:guid}")]
    [MapToApiVersion("1.0")]
    [Authorize]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [Consumes("application/json")]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetScheduleReportsV1")]
    [EndpointSummary("Get Summary of Schedule and you can download it.")]
    [EndpointDescription("Get Summary of Schedule and you can download it on your device include StartAt and EndAt.")]
    public async Task<IActionResult> GetScheduleReports(Guid UserId)
    {
        var report = await _workingHoursService.GetScheduleUserReports(UserId);
        return File(report, "text/csv", "ArticleSummary.csv");
    }
}
