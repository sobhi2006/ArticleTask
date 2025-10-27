using ArticleTask.Enums;

namespace ArticleTask.DTOs.Requests.Articles;

public class ArticleRequest
{
    public string? Title { get; set; }
    public string? Tags { get; set; }
    public string? Content { get; set; }
    public IFormFile? Image { get; set; }
    public ArticleCategory Category { get; set; }
    public ArticlePublishStatus PublishStatus { get; set; }
    public DateTime PublishedAt{ get; set; }
}