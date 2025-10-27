using ArticleTask.Entities;
using ArticleTask.Enums;

namespace ArticleTask.DTOs.Responses.Articles;

public class ArticleResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Tags { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public ArticleCategory Category { get; set; }
    public ArticlePublishStatus PublishStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime PublishedAt { get; set; }
    public Guid CreatedBy { get; set; }
    private ArticleResponse() { }
    public static ArticleResponse FromModule(Article article)
    {
        ArticleResponse response = new()
        {
            Id = article.Id,
            Title = article.Title,
            Content = article.Content,
            Category = article.Category,
            ImageUrl = article.ImageUrl,
            PublishStatus = article.PublishStatus,
            Tags = article.Tags,
            CreatedAt = article.CreatedAt,
            PublishedAt = article.PublishedAt,
            CreatedBy = (Guid)article.CreatedBy!
        };
        return response;
    }
}