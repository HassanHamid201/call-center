using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Specialty> Specialties => Set<Specialty>();
    public DbSet<Sector> Sectors => Set<Sector>();
    public DbSet<User> Users => Set<User>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Seed data
        var branchId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var specialtyId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var sectorId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<Branch>().HasData(
            new Branch
            {
                Id = branchId,
                Name = "Tadawi Main Hospital",
                Address = "123 Healthcare Ave",
                City = "Dubai",
                Phone = "+971-4-123-4567",
                Email = "main@tadawi.med",
                Services = new List<string> { "Emergency", "Outpatient", "Surgery", "Radiology" },
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Branch
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111112"),
                Name = "Tadawi Clinic - Marina",
                Address = "456 Marina Walk",
                City = "Dubai",
                Phone = "+971-4-987-6543",
                Email = "marina@tadawi.med",
                Services = new List<string> { "Outpatient", "Dental", "Physiotherapy" },
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Specialty>().HasData(
            new Specialty
            {
                Id = specialtyId,
                Name = "Cardiology",
                Description = "Heart and cardiovascular system",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222223"),
                Name = "Dentistry",
                Description = "Dental and oral health",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Sector>().HasData(
            new Sector
            {
                Id = sectorId,
                Name = "Adult Medicine",
                Description = "Healthcare services for adults",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Sector
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333334"),
                Name = "Pediatrics",
                Description = "Healthcare services for children",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Doctor>().HasData(
            new Doctor
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                FirstName = "Ahmed",
                LastName = "Hassan",
                Email = "a.hassan@tadawi.med",
                Phone = "+971-50-111-2222",
                Mobile = "+971-50-111-2222",
                Languages = new List<string> { "Arabic", "English" },
                Qualifications = "MD, FACC",
                Biography = "Senior cardiologist with 15 years experience",
                IsActive = true,
                BranchId = branchId,
                SpecialtyId = specialtyId,
                SectorId = sectorId,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444445"),
                FirstName = "Sarah",
                LastName = "Johnson",
                Email = "s.johnson@tadawi.med",
                Phone = "+971-50-333-4444",
                Mobile = "+971-50-333-4444",
                Languages = new List<string> { "English", "French" },
                Qualifications = "BDS, MOrth",
                Biography = "Pediatric dentist specializing in early orthodontic care",
                IsActive = true,
                BranchId = Guid.Parse("11111111-1111-1111-1111-111111111112"),
                SpecialtyId = Guid.Parse("22222222-2222-2222-2222-222222222223"),
                SectorId = Guid.Parse("33333333-3333-3333-3333-333333333334"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Email = "admin@tadawi.med",
                PasswordHash = "$2a$11$xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", // placeholder
                FirstName = "System",
                LastName = "Admin",
                Role = Enums.UserRole.Admin,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
