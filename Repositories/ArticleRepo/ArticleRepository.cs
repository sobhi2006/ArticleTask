using System.Dynamic;
using System.Transactions;
using ArticleTask.Data;
using ArticleTask.Entities;
using ArticleTask.Enums;
using Microsoft.EntityFrameworkCore;

namespace ArticleTask.Repositories.ArticleRepo;

public class ArticleRepository(AppDbContext context) : IArticleRepository
{
    readonly AppDbContext _context = context;
    public async Task<bool> AddNewArticle(Article article)
    {
        await _context.Articles.AddAsync(article);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task DeleteArticle(Guid Id)
    {
        var Article = await _context.Articles.Where(a => a.Id == Id).FirstOrDefaultAsync();
        _context.Articles.Remove(Article!);
        await _context.SaveChangesAsync();
        this.DeleteImage(Article!.ImageUrl);
    }

    public async Task<bool> ExistArticle(Guid ArticleId)
    {
        return await _context.Articles.AnyAsync(a => a.Id == ArticleId);
    }

    public async Task<bool> ExistArticle(string Title, string Content, Guid Id)
    {
        return await _context.Articles.AnyAsync(a => a.Title == Title && a.Content == Content && a.Id != Id);
    }

    public async Task<Article?> GetArticleById(Guid Id)
    {
        var Article = await _context.Articles.AsNoTracking()
                                             .Where(a => a.Id == Id)
                                             .FirstOrDefaultAsync();
        return Article;
    }

    public async Task<IEnumerable<Article>?> GetArticleByTitle(string Title, int Page, int SizePage)
    {
        var Articles = await _context.Articles.AsNoTracking()
                                        .Where(a => a.Title == Title)
                                        .Skip((Page - 1) * SizePage)
                                        .Take(SizePage)
                                        .ToListAsync();
        return Articles;
    }

    public async Task<IEnumerable<Article>?> GetArticleByUserId(Guid UserId, int Page, int SizePage)
    {
        var Articles = await _context.Articles.AsNoTracking()
                                        .Where(a => a.UserId == UserId)
                                        .Skip((Page - 1) * SizePage)
                                        .Take(SizePage)
                                        .ToListAsync();
        return Articles;
    }

    public bool DeleteImage(string imageUrl)
    {
        try
        {
            var fileName = Path.GetFileName(imageUrl);
            var ArticleName = Path.GetFileName(Path.GetDirectoryName(imageUrl));
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Articles", ArticleName!, fileName);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<Article>?> GetArticlesOrderByTitle(int Page, int SizePage)
    {
        var Articles = await _context.Articles.OrderBy(a => a.Title)
                                              .Skip((Page - 1) * SizePage)
                                              .Take(SizePage)
                                              .ToListAsync();
        return Articles;
    }

    public async Task<IEnumerable<Article>?> GetArticlesByRangeDate(int Page, int SizePage, DateTime Start, DateTime End)
    {
        var Articles = await _context.Articles.Where(a => a.CreatedAt >= Start && a.CreatedAt <= End &&
                                                     a.PublishedAt <= DateTime.UtcNow)
                                              .Skip((Page - 1) * SizePage)
                                              .Take(SizePage)
                                              .ToListAsync();
        return Articles;
    }

    public async Task<int> GetNumArticlesByCategory(ArticleCategory category)
    {
        int num = await _context.Articles.CountAsync(a => a.Category == category);
        return num;
    }

    public IEnumerable<dynamic>? GetArticlesSummaryReports()
    {
        var Articles = _context.Articles.Select(a => new { a.Title, a.Category }).ToList();
        return Articles.Select(a =>
        {
            dynamic obj = new ExpandoObject();
            obj.Title = a.Title;
            obj.Category = a.Category;
            return obj;
        });
    }
}