namespace ArticleTask.ExceptionHandlers;

public class BusinessException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; set; } = statusCode;
}