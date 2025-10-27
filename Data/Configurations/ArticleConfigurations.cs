using ArticleTask.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArticleTask.Data.Configurations;

public class ArticleConfigurations : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
       builder.ToTable("Articles");

       builder.Property(a => a.Category)
              .HasConversion<string>();

       builder.Property(a => a.PublishStatus)
              .HasConversion<string>();

       builder.Property(a => a.Content)
              .HasColumnType("NVARCHAR(MAX)");

       builder.Property(a => a.Title)
              .HasColumnType("NVARCHAR(20)");

       builder.Property(a => a.Tags)
              .HasColumnType("NVARCHAR(20)");

       builder.Property(a => a.ImageUrl).HasColumnType("NVARCHAR(100)");

       builder.HasOne<User>(a => a.User)
              .WithMany(u => u.Articles)
              .HasForeignKey(a => a.UserId);

       builder.HasIndex(a => a.Title);
    }
}
