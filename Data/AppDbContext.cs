using ArticleTask.Data.Configurations;
using ArticleTask.Entities;
using ArticleTask.Entities.WorkingHoursManagement;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArticleTask.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options),
IDataProtectionKeyContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();
    public DbSet<UserWorkingHours> UserWorkingHours => Set<UserWorkingHours>();
    public DbSet<WorkingHoursAtDay> WorkingHoursAtDays => Set<WorkingHoursAtDay>();
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfigurations).Assembly);
    }
}