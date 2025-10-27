using ArticleTask.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArticleTask.Data.Configurations;

public class UserPreferencesConfigurations : IEntityTypeConfiguration<UserPreference>
{
    public void Configure(EntityTypeBuilder<UserPreference> builder)
    {
        builder.ToTable("UserPreferences");

        builder.Property(up => up.Language)
               .HasConversion<string>()
               .HasColumnType("NVARCHAR(20)");

        builder.Property(up => up.Theme)
               .HasConversion<string>()
               .HasColumnType("NVARCHAR(20)");;
    }
}
