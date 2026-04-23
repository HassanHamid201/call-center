using Domain.Entities.Reference;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Contexts;

public class ReferenceDbContext : DbContext
{
    public ReferenceDbContext(DbContextOptions<ReferenceDbContext> options) : base(options) { }

    public DbSet<Nationality> Nationalities => Set<Nationality>();
    public DbSet<InsuranceOption> InsuranceOptions => Set<InsuranceOption>();
    public DbSet<Classification> Classifications => Set<Classification>();
    public DbSet<AvailabilityStatus> AvailabilityStatuses => Set<AvailabilityStatus>();
    public DbSet<ClinicMechanism> ClinicMechanisms => Set<ClinicMechanism>();
    public DbSet<Coordinator> Coordinators => Set<Coordinator>();
    public DbSet<WorkingHour> WorkingHours => Set<WorkingHour>();
    public DbSet<WorkingDay> WorkingDays => Set<WorkingDay>();
    public DbSet<AgeGroup> AgeGroups => Set<AgeGroup>();
    public DbSet<ServiceCatalog> Services => Set<ServiceCatalog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        SeedData(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Nationality>().HasData(
            new Nationality { Id = Guid.Parse("10000001-0001-0001-0001-000000000001"), Name = "سعودي", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Nationality { Id = Guid.Parse("10000001-0001-0001-0001-000000000002"), Name = "مصري", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Nationality { Id = Guid.Parse("10000001-0001-0001-0001-000000000003"), Name = "سوداني", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Nationality { Id = Guid.Parse("10000001-0001-0001-0001-000000000004"), Name = "سوري", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Nationality { Id = Guid.Parse("10000001-0001-0001-0001-000000000005"), Name = "فلسطيني", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<InsuranceOption>().HasData(
            new InsuranceOption { Id = Guid.Parse("10000002-0002-0002-0002-000000000001"), Name = "يقبل تأمين", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new InsuranceOption { Id = Guid.Parse("10000002-0002-0002-0002-000000000002"), Name = "لا يقبل تأمين", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new InsuranceOption { Id = Guid.Parse("10000002-0002-0002-0002-000000000003"), Name = "الاطلاع عالملاحظه", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<Classification>().HasData(
            new Classification { Id = Guid.Parse("10000003-0003-0003-0003-000000000001"), Name = "طبيب عام", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Classification { Id = Guid.Parse("10000003-0003-0003-0003-000000000002"), Name = "أخصائي", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Classification { Id = Guid.Parse("10000003-0003-0003-0003-000000000003"), Name = "أستشاري", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Classification { Id = Guid.Parse("10000003-0003-0003-0003-000000000004"), Name = "بروفيسور", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<AvailabilityStatus>().HasData(
            new AvailabilityStatus { Id = Guid.Parse("10000004-0004-0004-0004-000000000001"), Name = "متواجد", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new AvailabilityStatus { Id = Guid.Parse("10000004-0004-0004-0004-000000000002"), Name = "إجازة", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new AvailabilityStatus { Id = Guid.Parse("10000004-0004-0004-0004-000000000003"), Name = "مكتفية", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new AvailabilityStatus { Id = Guid.Parse("10000004-0004-0004-0004-000000000004"), Name = "تنبية", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new AvailabilityStatus { Id = Guid.Parse("10000004-0004-0004-0004-000000000005"), Name = "لايوجد عيادة", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<ClinicMechanism>().HasData(
            new ClinicMechanism { Id = Guid.Parse("10000005-0005-0005-0005-000000000001"), Name = "الأولوية بقص الفاتورة", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ClinicMechanism { Id = Guid.Parse("10000005-0005-0005-0005-000000000002"), Name = "بالمواعيد", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ClinicMechanism { Id = Guid.Parse("10000005-0005-0005-0005-000000000003"), Name = "بالمواعيد والأرقام", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ClinicMechanism { Id = Guid.Parse("10000005-0005-0005-0005-000000000004"), Name = "توزيع الارقام قبل العياده بيوم من قسم خدمة العملاء", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<Coordinator>().HasData(
            new Coordinator { Id = Guid.Parse("10000006-0006-0006-0006-000000000001"), Name = "ساره عسيري", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Coordinator { Id = Guid.Parse("10000006-0006-0006-0006-000000000002"), Name = "منيره الشهري", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Coordinator { Id = Guid.Parse("10000006-0006-0006-0006-000000000003"), Name = "منى الشهري", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Coordinator { Id = Guid.Parse("10000006-0006-0006-0006-000000000004"), Name = "ملاذ الشهراني", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<WorkingHour>().HasData(
            new WorkingHour { Id = Guid.Parse("10000007-0007-0007-0007-000000000001"), Name = "10AM-6PM", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new WorkingHour { Id = Guid.Parse("10000007-0007-0007-0007-000000000002"), Name = "9AM-12PM - 4PM-9PM", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new WorkingHour { Id = Guid.Parse("10000007-0007-0007-0007-000000000003"), Name = "غير ثابت", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new WorkingHour { Id = Guid.Parse("10000007-0007-0007-0007-000000000004"), Name = "فترتين", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<WorkingDay>().HasData(
            new WorkingDay { Id = Guid.Parse("10000008-0008-0008-0008-000000000001"), Name = "من السبت الى الخميس", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new WorkingDay { Id = Guid.Parse("10000008-0008-0008-0008-000000000002"), Name = "من الاحد الى الخميس", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new WorkingDay { Id = Guid.Parse("10000008-0008-0008-0008-000000000003"), Name = "يومياً عدا الجمعة", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new WorkingDay { Id = Guid.Parse("10000008-0008-0008-0008-000000000004"), Name = "السبت - الأحد - الثلاثاء - الأربعاء", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<AgeGroup>().HasData(
            new AgeGroup { Id = Guid.Parse("10000009-0009-0009-0009-000000000001"), Name = "جميع الفئات العمرية", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new AgeGroup { Id = Guid.Parse("10000009-0009-0009-0009-000000000002"), Name = "فوق 13 سنه", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new AgeGroup { Id = Guid.Parse("10000009-0009-0009-0009-000000000003"), Name = "من 18 سنه واقل", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new AgeGroup { Id = Guid.Parse("10000009-0009-0009-0009-000000000004"), Name = "الاطفال - الكبار", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new AgeGroup { Id = Guid.Parse("10000009-0009-0009-0009-000000000005"), Name = "كبار فقط", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<ServiceCatalog>().HasData(
            new ServiceCatalog { Id = Guid.Parse("10000010-0010-0010-0010-000000000001"), Name = "كشف طبي", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ServiceCatalog { Id = Guid.Parse("10000010-0010-0010-0010-000000000002"), Name = "أشعة سونار", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ServiceCatalog { Id = Guid.Parse("10000010-0010-0010-0010-000000000003"), Name = "مختبر تحاليل", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ServiceCatalog { Id = Guid.Parse("10000010-0010-0010-0010-000000000004"), Name = "علاج طبيعي", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new ServiceCatalog { Id = Guid.Parse("10000010-0010-0010-0010-000000000005"), Name = "استشارة تغذية", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
