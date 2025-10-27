using ArticleTask.Entities;
using ArticleTask.Enums;

namespace ArticleTask.Repositories.ArticleRepo;

public interface IArticleRepository
{
    public Task<bool> AddNewArticle(Article request);
    public Task<bool> ExistArticle(Guid ArticleId);
    public Task<bool> ExistArticle(string Title, string Content, Guid Id);
    public Task DeleteArticle(Guid Id);
    public Task<Article?> GetArticleById(Guid Id);
    public Task<IEnumerable<Article>?> GetArticleByUserId(Guid UserId, int Page, int SizePage);
    public Task<IEnumerable<Article>?> GetArticleByTitle(string Title, int Page, int SizePage);
    public Task<IEnumerable<Article>?> GetArticlesOrderByTitle(int Page, int SizePage);
    public Task<IEnumerable<Article>?> GetArticlesByRangeDate(int Page, int SizePage, DateTime Start, DateTime End);
    public Task<int> GetNumArticlesByCategory(ArticleCategory category);
    public IEnumerable<dynamic>? GetArticlesSummaryReports();
    public bool DeleteImage(string imageUrl);

}