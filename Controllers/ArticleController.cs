using System.Security.Claims;
using ArticleTask.DTOs.Requests.Articles;
using ArticleTask.DTOs.Responses.Articles;
using ArticleTask.Enums;
using ArticleTask.Services.ArticleService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ArticleTask.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[Controller]")]
[ApiVersion("1.0")]
[Tags("Articles")]
public class ArticleController(IArticleService _articleService) : ControllerBase
{
    private Guid _currentUserId
        => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [MapToApiVersion("1.0")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType<ActionResult<ArticleResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ArticleResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("AddNewArticleAsyncV1")]
    [EndpointSummary("Creates a new Article")]
    [EndpointDescription("Creates a new Article and returns the created result.")]
    public async Task<ActionResult<ArticleResponse>> AddNewArticleAsync([FromForm] ArticleRequest request)
    {
        var Article = await _articleService.AddNewArticle(request, _currentUserId);
        return Created("Article added successfully", Article);
    }

    [HttpPut("{articleId:guid}")]
    [Authorize]
    [MapToApiVersion("1.0")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType<ActionResult<ArticleResponse>>(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ArticleResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UpdateArticleAsyncV1")]
    [EndpointSummary("Update a Article")]
    [EndpointDescription("Update a Article and returns the updated result.")]
    public async Task<ActionResult<ArticleResponse>> UpdateArticleAsync(Guid articleId, [FromForm] ArticleRequest request)
    {
        await _articleService.UpdateArticle(articleId, request, _currentUserId);
        return NoContent();
    }

    [HttpDelete("{articleId:guid}")]
    [Authorize(Roles = "Admin")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ArticleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ArticleResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("DeleteArticleAsyncV1")]
    [EndpointSummary("Delete a Article by id")]
    [EndpointDescription("Delete a Article and returns the deleted result.")]
    public async Task<ActionResult> DeleteArticleAsync(Guid articleId)
    {
        await _articleService.DeleteArticle(articleId);
        return Ok("Article deleted successfully");
    }

    [HttpGet("title/{title}")]
    [MapToApiVersion("1.0")]
    [Authorize]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<ArticleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetArticlesByTitleAsyncV1")]
    [EndpointSummary("Retrieves Articles by status")]
    [EndpointDescription("Retrieves Articles by status and returns the retrieve result.")]
    public async Task<ActionResult<List<ArticleResponse>>> GetArticlesByTitleAsync(string title, [FromQuery] int Page = 1, [FromQuery] int PageSize = 10)
    {
        if (string.IsNullOrEmpty(title))
            return BadRequest("title is required");
        Page = Math.Max(1, Page);
        PageSize = Math.Clamp(PageSize, 1, 100);
        var Articles = await _articleService.GetArticleByTitle(title, Page, PageSize);
        return Ok(Articles);
    }

    [HttpGet("user/{userId:guid}")]
    [MapToApiVersion("1.0")]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<ArticleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetArticlesByUserIdAsyncV1")]
    [EndpointSummary("Retrieves Articles by UserId")]
    [EndpointDescription("Retrieves Articles by UserId and returns the retrieve result.")]
    public async Task<ActionResult<List<ArticleResponse>>> GetArticlesByUserIdAsync(Guid userId, [FromQuery] int Page = 1, [FromQuery] int PageSize = 10)
    {
        Page = Math.Max(1, Page);
        PageSize = Math.Clamp(PageSize, 1, 100);
        var Articles = await _articleService.GetArticleByUserId(userId, Page, PageSize);
        return Ok(Articles);
    }

    [HttpGet("{id:guid}")]
    [MapToApiVersion("1.0")]
    [Authorize]
    [Consumes("application/json")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [ProducesResponseType<ActionResult<ArticleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetArticlesByIdAsyncV1")]
    [EndpointSummary("Retrieves Articles by id")]
    [EndpointDescription("Retrieves Articles by id and returns the retrieve result.")]
    public async Task<ActionResult<List<ArticleResponse>>> GetArticlesByIdAsync(Guid id)
    {
        var Articles = await _articleService.GetArticleById(id);
        return Ok(Articles);
    }

    [HttpGet("num/{category}")]
    [MapToApiVersion("1.0")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<ArticleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetNumArticlesByCategoryAsyncV1")]
    [EndpointSummary("Retrieves num Articles By Category")]
    [EndpointDescription("Retrieves num Articles By Category and returns the retrieve result.")]
    public async Task<ActionResult<int>> GetNumArticlesAsync(ArticleCategory category)
    {
        var Num = await _articleService.GetNumArticlesByCategory(category);
        return Ok(Num);
    }

    [HttpGet("Range/{Start:datetime}/{End:datetime}")]
    [MapToApiVersion("1.0")]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<ArticleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetArticlesByRangeDateAsyncV1")]
    [EndpointSummary("Retrieves Articles ByRangeDate")]
    [EndpointDescription("Retrieves Articles ByRangeDate and returns the retrieve result.")]
    public async Task<ActionResult<List<ArticleResponse>>> GetArticlesByRangeDateAsync(DateTime Start, DateTime End,
                                                           [FromQuery] int Page = 1, [FromQuery] int PageSize = 10)
    {
        Page = Math.Max(1, Page);
        PageSize = Math.Clamp(PageSize, 1, 100);
        var Articles = await _articleService.GetArticlesByRangeDate(Page, PageSize, Start, End);
        return Ok(Articles);
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<ArticleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetArticlesOrderByTitleAsyncV1")]
    [EndpointSummary("Retrieves Articles OrderByTitle")]
    [EndpointDescription("Retrieves Articles OrderByTitle and returns the retrieve result.")]
    public async Task<ActionResult<List<ArticleResponse>>> GetArticlesOrderByTitleAsync([FromQuery] int Page = 1, [FromQuery] int PageSize = 10)
    {
        Page = Math.Max(1, Page);
        PageSize = Math.Clamp(PageSize, 1, 100);
        var Articles = await _articleService.GetArticlesOrderByTitle(Page, PageSize);
        return Ok(Articles);
    }
    
    [HttpGet("Reports")]
    [MapToApiVersion("1.0")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [Consumes("application/json")]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetArticlesReportsV1")]
    [EndpointSummary("Get Summary of Article and you can download it.")]
    [EndpointDescription("Get Summary of Article and you can download it on your device include title and category.")]
    public IActionResult GetArticlesReports()
    {
        var report = _articleService.GetArticlesSummaryReports();
        return File(report, "text/csv", "ArticleSummary.csv");
    }
}
