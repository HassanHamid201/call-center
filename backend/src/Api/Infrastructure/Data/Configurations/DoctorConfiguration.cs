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
        builder.Property(d => d.DisplayName).HasMaxLength(200);
        builder.Property(d => d.Nationality).HasMaxLength(50);
        builder.Property(d => d.Classification).HasMaxLength(50);
        builder.Property(d => d.InsuranceAcceptance).HasMaxLength(100);
        builder.Property(d => d.AvailabilityStatus).HasMaxLength(50);
        builder.Property(d => d.CoordinatorName).HasMaxLength(100);
        builder.Property(d => d.InternalExtension).HasMaxLength(50);
        builder.Property(d => d.WorkingHours).HasMaxLength(100);
        builder.Property(d => d.WorkingDays).HasMaxLength(100);
        builder.Property(d => d.AgeGroup).HasMaxLength(100);
        builder.Property(d => d.ClinicMechanism).HasMaxLength(200);
        builder.HasIndex(d => d.LastName);
        builder.HasIndex(d => d.IsActive);
        builder.HasIndex(d => d.Classification);
        builder.HasIndex(d => d.AvailabilityStatus);
        builder.HasIndex(d => d.BranchId);

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
