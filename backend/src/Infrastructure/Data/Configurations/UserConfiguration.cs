using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.IsActive);

        builder.HasOne(u => u.Branch)
            .WithMany()
            .HasForeignKey(u => u.BranchId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
