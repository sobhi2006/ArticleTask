using ArticleTask.Entities.WorkingHoursManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArticleTask.Data.Configurations;

public class WorkingHoursAtDayConfigurations : IEntityTypeConfiguration<WorkingHoursAtDay>
{
    public void Configure(EntityTypeBuilder<WorkingHoursAtDay> builder)
    {
        builder.ToTable("WorkingHoursAtDay");

        builder.HasOne<UserWorkingHours>(whd => whd.WorkingHours)
               .WithMany(wh => wh.WorkingHoursInDay)
               .HasForeignKey(whd => whd.WorkingHoursId);

        builder.OwnsOne(whd => whd.TimeSlot, TimeSlot =>
        {
            TimeSlot.Property(t => t.StartAt)
                    .HasColumnName("StartAt");

            TimeSlot.Property(t => t.EndAt)
                    .HasColumnName("EndAt");
        });
    }
}
