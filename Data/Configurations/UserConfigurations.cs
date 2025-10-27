using ArticleTask.Entities;
using ArticleTask.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArticleTask.Data.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
       builder.ToTable("Users");

       builder.Property(u => u.FName)
              .HasColumnType("NVARCHAR(50)");

       builder.Property(u => u.LName)
              .HasColumnType("NVARCHAR(50)");

       builder.Property(u => u.Email)
              .HasColumnType("NVARCHAR(100)");

       builder.Property(u => u.Password)
              .HasColumnType("NVARCHAR(200)");
              
       builder.Property(u => u.CreatedBy)
              .IsRequired(false);

       builder.HasIndex(u => u.Email);

       builder.HasData([new User()
       {
              Id = Guid.Parse("A5FC9A37-F31A-43B4-AAD8-446880DBD6EB"),
              DateOfBirth = new DateTime(2006, 4, 1),
              FName = "Sobhi",
              LName = "Hazouri",
              CreatedBy = null,
              Email = "sobhihazouri2006@gmail.com",
              Password = "CfDJ8PI5dPR7ADFDkcz2tsJhFxgDZwDRGDv0Sx77qOJV0mkxShw7SIFmM1CtBmHslPJ66Lv2AWjZVLrJCtDytgmaPonpef9_8Dr4v9qba2QKCPwMgxAtBvpzxPbpjCAz-8Lzmw", //     Admin1122@      :     is password
              RefreshToken = null,
              Expired = DateTime.MinValue,
              Role = UserRole.Admin
       }]);
    }
}
