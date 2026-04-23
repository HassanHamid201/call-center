using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(d => d.LastName).IsRequired().HasMaxLength(100);
        builder.Property(d => d.Email).HasMaxLength(200);
        builder.Property(d => d.Phone).HasMaxLength(50);
        builder.Property(d => d.Mobile).HasMaxLength(50);
        // SQLite stores arrays as JSON automatically
        builder.Property(d => d.Qualifications).HasMaxLength(500);
        builder.HasIndex(d => d.LastName);
        builder.HasIndex(d => d.IsActive);

        builder.HasOne(d => d.Branch)
            .WithMany(b => b.Doctors)
            .HasForeignKey(d => d.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Specialty)
            .WithMany(s => s.Doctors)
            .HasForeignKey(d => d.SpecialtyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Sector)
            .WithMany(s => s.Doctors)
            .HasForeignKey(d => d.SectorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
