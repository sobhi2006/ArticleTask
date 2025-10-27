using System.Text;
using System.Transactions;
using ArticleTask.DTOs.Requests.Articles;
using ArticleTask.DTOs.Responses.Articles;
using ArticleTask.Entities;
using ArticleTask.Enums;
using ArticleTask.ExceptionHandlers;
using ArticleTask.Repositories.ArticleRepo;

namespace ArticleTask.Services.ArticleService;

public class ArticleService(IArticleRepository articleRepository) : IArticleService
{
    readonly IArticleRepository _articleRepository = articleRepository;
    public async Task<bool> AddNewArticle(ArticleRequest request, Guid UserId)
    {
        if (await _articleRepository.ExistArticle(request.Title!, request.Content!, Guid.NewGuid()))
            throw new BusinessException("Article already has found", StatusCodes.Status400BadRequest);

        try
        {
            Article NewArticle = new()
            {
                Id = Guid.NewGuid(),
                Title = request.Title!,
                Content = request.Content!,
                Category = request.Category,
                CreatedBy = UserId,
                CreatedAt = DateTime.UtcNow,
                PublishedAt = request.PublishedAt,
                PublishStatus = request.PublishStatus,
                Tags = request.Tags!,
                ImageUrl = await this.SaveImagesAsync(request.Image!, request.Title!),
                UserId = UserId
            };
            if (!await _articleRepository.AddNewArticle(NewArticle))
                _articleRepository.DeleteImage(NewArticle.ImageUrl);
        }
        catch (Exception ex)
        {
            throw new BusinessException(ex.Message, StatusCodes.Status500InternalServerError);
        }
        return true;
    }

    public async Task DeleteArticle(Guid Id)
    {
        if (!await _articleRepository.ExistArticle(Id))
            throw new BusinessException("Article has not found to delete", StatusCodes.Status400BadRequest);

        await _articleRepository.DeleteArticle(Id);
    }

    public async Task<ArticleResponse> GetArticleById(Guid Id)
    {
        var Article = await _articleRepository.GetArticleById(Id) ??
            throw new BusinessException("Article has not found", StatusCodes.Status400BadRequest);

        return ArticleResponse.FromModule(Article);
    }

    public async Task<IEnumerable<ArticleResponse>?> GetArticleByTitle(string Title, int Page, int SizePage)
    {
        var Articles = await _articleRepository.GetArticleByTitle(Title, Page, SizePage) ??
            throw new BusinessException("Articles has not found", StatusCodes.Status400BadRequest);

        return Articles.Select(ArticleResponse.FromModule);
    }

    public async Task<IEnumerable<ArticleResponse>?> GetArticleByUserId(Guid UserId, int Page, int SizePage)
    {
        var Articles = await _articleRepository.GetArticleByUserId(UserId, Page, SizePage) ??
            throw new BusinessException("Articles has not found", StatusCodes.Status400BadRequest);

        return Articles.Select(ArticleResponse.FromModule);
    }

    public async Task UpdateArticle(Guid Id, ArticleRequest request, Guid UserId)
    {
        var article = await _articleRepository.GetArticleById(Id) ??
            throw new BusinessException("Articles has not found", StatusCodes.Status400BadRequest);

        if (await _articleRepository.ExistArticle(request.Title!, request.Content!, Id))
            throw new BusinessException("Conflict with another article", StatusCodes.Status409Conflict);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await this.DeleteArticle(Id);
        await this.AddNewArticle(request, UserId);

        transaction.Complete();
    }
    
    public async Task<string> SaveImagesAsync(IFormFile image, string Title)
    {
        var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var imagesPath = Path.Combine(webRootPath, "images", "Articles", Title);

        if (!Directory.Exists(imagesPath))
            Directory.CreateDirectory(imagesPath);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var filePath = Path.Combine(imagesPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await image.CopyToAsync(stream);

        return $"/images/Articles/{Title}/{fileName}";
    }

    public async Task<IEnumerable<ArticleResponse>?> GetArticlesOrderByTitle(int Page, int SizePage)
    {
        var Articles = await _articleRepository.GetArticlesOrderByTitle(Page, SizePage) ??
            throw new BusinessException("Articles has not found", StatusCodes.Status400BadRequest);

        return Articles.Select(ArticleResponse.FromModule);
    }

    public async Task<IEnumerable<ArticleResponse>?> GetArticlesByRangeDate(int Page, int SizePage, DateTime Start, DateTime End)
    {
        var Articles = await _articleRepository.GetArticlesByRangeDate(Page, SizePage, Start, End) ??
            throw new BusinessException("Articles has not found", StatusCodes.Status400BadRequest);

        return Articles.Select(ArticleResponse.FromModule);
    }

    public async Task<int> GetNumArticlesByCategory(ArticleCategory category)
    {
        int num = await _articleRepository.GetNumArticlesByCategory(category);
        return num;
    }

    public byte[] GetArticlesSummaryReports()
    {
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("\t\t\t\tSummary Of Articles");
        csvBuilder.AppendLine();
        csvBuilder.AppendLine();

        var Articles = _articleRepository.GetArticlesSummaryReports() ??
            throw new BusinessException("Articles has not found", StatusCodes.Status400BadRequest);

        int RowNumber = 1;
        foreach (var article in Articles)
        {
            csvBuilder.AppendLine($"{RowNumber}-) {article.Title} {article.Category}");
            csvBuilder.AppendLine();
            RowNumber++;
        }
        csvBuilder.AppendLine("\t\t\t\tTHE END");

        var fileBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

        return fileBytes;
    }
}