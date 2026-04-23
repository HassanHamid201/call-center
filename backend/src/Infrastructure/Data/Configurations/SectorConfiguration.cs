using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class SectorConfiguration : IEntityTypeConfiguration<Sector>
{
    public void Configure(EntityTypeBuilder<Sector> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Description).HasMaxLength(1000);
        builder.HasIndex(s => s.IsActive);
    }
}
