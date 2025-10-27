using ArticleTask.Entities.Common;
using ArticleTask.Enums;

namespace ArticleTask.Entities;

public class Article : CommonFields
{
    public string Title { get; set; } = null!;
    public string Tags { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public ArticleCategory Category { get; set; }
    public ArticlePublishStatus PublishStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime PublishedAt { get; set; }
    public User User { get; set; } = null!;
    public Guid UserId { get; set; }
}