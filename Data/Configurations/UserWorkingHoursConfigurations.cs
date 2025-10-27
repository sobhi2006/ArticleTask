using ArticleTask.Entities;
using ArticleTask.Entities.WorkingHoursManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArticleTask.Data.Configurations;

public class UserWorkingHoursConfiguration : IEntityTypeConfiguration<UserWorkingHours>
{
    public void Configure(EntityTypeBuilder<UserWorkingHours> builder)
    {
        builder.ToTable("WorkingHours");

        builder.Property(wh => wh.Day)
               .HasConversion<string>();

        builder.HasOne<User>(wh => wh.User)
               .WithMany(u => u.WorkingHours)
               .HasForeignKey(wh => wh.UserId);
    }
}
