using ArticleTask.DTOs.Requests.Articles;
using ArticleTask.DTOs.Responses.Articles;
using ArticleTask.Enums;

namespace ArticleTask.Services.ArticleService;

public interface IArticleService
{
    public Task<bool> AddNewArticle(ArticleRequest request, Guid UserId);
    public Task UpdateArticle(Guid Id, ArticleRequest request, Guid UserId);
    public Task DeleteArticle(Guid Id);
    public Task<ArticleResponse> GetArticleById(Guid Id);
    public Task<IEnumerable<ArticleResponse>?> GetArticleByUserId(Guid UserId, int Page, int SizePage);
    public Task<IEnumerable<ArticleResponse>?> GetArticleByTitle(string Title, int Page, int SizePage);
    public Task<IEnumerable<ArticleResponse>?> GetArticlesOrderByTitle(int Page, int SizePage);
    public Task<IEnumerable<ArticleResponse>?> GetArticlesByRangeDate(int Page, int SizePage, DateTime Start, DateTime End);
    public Task<int> GetNumArticlesByCategory(ArticleCategory category);
    public byte[] GetArticlesSummaryReports();
}