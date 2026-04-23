using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class FaqItemConfiguration : IEntityTypeConfiguration<FaqItem>
{
    public void Configure(EntityTypeBuilder<FaqItem> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Question).IsRequired().HasMaxLength(500);
        builder.Property(f => f.Answer).IsRequired().HasMaxLength(2000);
        builder.Property(f => f.Category).HasMaxLength(100);
        builder.HasIndex(f => f.IsActive);
        builder.HasIndex(f => f.Category);
    }
}
