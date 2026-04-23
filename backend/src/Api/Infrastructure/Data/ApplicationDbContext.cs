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
    public DbSet<FaqItem> FaqItems => Set<FaqItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Seed Branches
        modelBuilder.Entity<Branch>().HasData(
            new Branch
            {
                Id = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                Name = "مجمع تداوي الطبي - فرع أبها",
                NameEn = "Tadawi Medical Complex - Abha",
                Address = "أبها، عسير",
                City = "أبها",
                Region = "عسير",
                Phone = "+966-17-2430-000",
                Services = new List<string> { "طوارئ 24H", "أشعة", "مختبر", "عيادات تخصصية", "جلدية وتجميل", "ليزر وإزالة شعر", "أسنان", "عيون", "أنف وأذن وحنجرة", "نساء وولادة", "أطفال", "غدد وسكري", "باطنية", "جراحة عامة", "عظام" },
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Branch
            {
                Id = Guid.Parse("75dd6d00-fc22-542d-af65-651f6135b394"),
                Name = "مستشفى تداوي الطبي TMH",
                NameEn = "Tadawi Medical Hospital",
                Address = "أبها، عسير",
                City = "أبها",
                Region = "عسير",
                Phone = "+966-17-5050-000",
                Services = new List<string> { "طوارئ 24H", "أشعة", "مختبر", "عيادات تخصصية", "جراحة", "عناية مركزة", "قسطرة", "تجميل" },
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Branch
            {
                Id = Guid.Parse("6960ef26-da8c-5d0a-807b-ce833a006ae5"),
                Name = "مجمع تداوي الطبي - فرع المصيف",
                NameEn = "Tadawi Medical Complex - Al-Mosaf",
                Address = "أبها، عسير",
                City = "أبها",
                Region = "عسير",
                Phone = "+966-17-3399-000",
                Services = new List<string> { "طوارئ", "أشعة", "مختبر", "عيادات تخصصية", "جلدية", "ليزر", "أسنان", "تجميل" },
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Branch
            {
                Id = Guid.Parse("b87e0359-603a-5d31-a76d-cbbe18d13a8e"),
                Name = "مجمع تداوي الطبي - فرع الموسى (الخميس)",
                NameEn = "Tadawi Medical Complex - Al-Mousa (Al-Khamis)",
                Address = "خميس مشيط، عسير",
                City = "خميس مشيط",
                Region = "عسير",
                Phone = "+966-17-1596-000",
                Services = new List<string> { "طوارئ 24H", "أشعة", "مختبر", "عيادات تخصصية", "جلدية", "ليزر وإزالة شعر", "أسنان", "عيون", "تجميل" },
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Branch
            {
                Id = Guid.Parse("b6b90fc4-3aea-5528-b9fb-bd185d09bc89"),
                Name = "مجمع تداوي الطبي - فرع جازان",
                NameEn = "Tadawi Medical Complex - Jazan",
                Address = "جازان، جازان",
                City = "جازان",
                Region = "جازان",
                Phone = "+966-17-4890-000",
                Services = new List<string> { "طوارئ", "أشعة", "مختبر", "عيادات تخصصية", "جلدية", "ليزر", "أسنان", "تجميل", "جراحة عامة" },
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Branch
            {
                Id = Guid.Parse("031f6a45-7d78-54b2-8bf0-b80c7a8ea289"),
                Name = "الري لايف - العلاج الطبيعي والتأهيل",
                NameEn = "Ray Life - Physiotherapy & Rehabilitation",
                Address = "أبها، عسير",
                City = "أبها",
                Region = "عسير",
                Phone = "+966-50-3388-000",
                Services = new List<string> { "علاج طبيعي", "تأهيل", "تخسيس", "تكسير دهون", "تدليك طبي", "باقات علاجية" },
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Branch
            {
                Id = Guid.Parse("f3c98415-4f36-5a99-a16d-d4d9da20d782"),
                Name = "الري لايف - مستشفى الرعاية LTC",
                NameEn = "Ray Life - Long Term Care Hospital",
                Address = "أبها، عسير",
                City = "أبها",
                Region = "عسير",
                Phone = "+966-50-3388-000",
                Services = new List<string> { "رعاية طويلة المدى", "علاج طبيعي", "تأهيل", "تمريض متخصص" },
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Seed Specialties
        modelBuilder.Entity<Specialty>().HasData(
            new Specialty
            {
                Id = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                Name = "طوارئ",
                Description = "خدمات طوارئ",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("5e2dd555-d7c9-52a7-ba1e-4f366ec82ce9"),
                Name = "باطنية",
                Description = "خدمات باطنية",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("037951d2-7e38-5fcb-b315-ea579ebaeabb"),
                Name = "قلبية",
                Description = "خدمات قلبية",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("c60f124e-2c16-58e5-9032-ac3e17c76761"),
                Name = "عظام",
                Description = "خدمات عظام",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("85bc473f-372b-568e-8bb0-cf028b27ed68"),
                Name = "جراحة عامة",
                Description = "خدمات جراحة عامة",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("3610f41b-4792-5aef-ba93-7b9c53f4d70b"),
                Name = "جلدية وتجميل",
                Description = "خدمات جلدية وتجميل",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("a0afe15e-4744-56e8-98ba-006d7e4f11f0"),
                Name = "عيون",
                Description = "خدمات عيون",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("1d753efb-6fc4-5afe-8e17-177e380ebeca"),
                Name = "أنف وأذن وحنجرة",
                Description = "خدمات أنف وأذن وحنجرة",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("bad82437-fe11-5e3f-b01f-9075b76577ce"),
                Name = "نساء وولادة",
                Description = "خدمات نساء وولادة",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("970a8508-ae8b-5af6-9f01-be501358496d"),
                Name = "أطفال",
                Description = "خدمات أطفال",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("cd76245f-512e-5f8e-8025-ee81d1619b8b"),
                Name = "أسنان",
                Description = "خدمات أسنان",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("75a9aa57-524d-5614-97e6-db9ff5eefd94"),
                Name = "غدد وسكري وسمنة",
                Description = "خدمات غدد وسكري وسمنة",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("a0d2d304-baf9-596f-9453-a09ab05e43a4"),
                Name = "مخ وأعصاب",
                Description = "خدمات مخ وأعصاب",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("ab7ff0bf-f015-5f0e-9180-c8fc1cf6c687"),
                Name = "صدرية",
                Description = "خدمات صدرية",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("3a71538b-f603-5920-a0b2-84359fbb4934"),
                Name = "مسالك بولية",
                Description = "خدمات مسالك بولية",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("f305ff07-c66b-5541-9f32-8ca9f61d047b"),
                Name = "كلى",
                Description = "خدمات كلى",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("f4621a7a-5a7c-551a-9d51-f713c6e2b686"),
                Name = "مناعة",
                Description = "خدمات مناعة",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("a2158ec6-fa30-58bc-953a-341cb9dd5bb6"),
                Name = "تخدير",
                Description = "خدمات تخدير",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("c34700d8-bcd4-599c-8c9c-1632f05f9a11"),
                Name = "أشعة تشخيصية",
                Description = "خدمات أشعة تشخيصية",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("0d22b65b-2439-5449-ad59-a07f66e89735"),
                Name = "مختبر",
                Description = "خدمات مختبر",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("dca18680-e84b-5344-9447-7c324432f221"),
                Name = "تغذية علاجية",
                Description = "خدمات تغذية علاجية",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("e8801033-2121-5f7c-ac4c-eece212dee1b"),
                Name = "نفسية",
                Description = "خدمات نفسية",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("bce7d899-220c-5fa1-8530-7240ca292dda"),
                Name = "علاج طبيعي وتأهيل",
                Description = "خدمات علاج طبيعي وتأهيل",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("f238e900-2bb4-5502-ace8-f1d92c558fc7"),
                Name = "تجميل",
                Description = "خدمات تجميل",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("1bc6d874-82bd-56fb-a274-52de8ae0197a"),
                Name = "ليزر وإزالة شعر",
                Description = "خدمات ليزر وإزالة شعر",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("6dfcc0d9-8f54-5c2b-9a88-db563a2e1d59"),
                Name = "سمعيات",
                Description = "خدمات سمعيات",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("96d390f4-ce8c-5ad2-9be2-55c7ae2aeb91"),
                Name = "تقويم أسنان",
                Description = "خدمات تقويم أسنان",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("a5419fa6-ad0f-5327-92c3-a412ebf3ec7d"),
                Name = "جراحة فم وفكين",
                Description = "خدمات جراحة فم وفكين",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("aa24ceca-e18c-5572-a634-4931b1bebd08"),
                Name = "علاج عصب الأسنان",
                Description = "خدمات علاج عصب الأسنان",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Specialty
            {
                Id = Guid.Parse("2b050a73-ca03-54a4-a3a3-b44d4b2fead9"),
                Name = "طب أسرة",
                Description = "خدمات طب أسرة",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Seed Sectors
        modelBuilder.Entity<Sector>().HasData(
            new Sector
            {
                Id = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                Name = "العيادات الطبية",
                Description = "العيادات التخصصية والطب العام",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Sector
            {
                Id = Guid.Parse("e11f5c92-8205-5c51-a663-04197c5a2322"),
                Name = "الجراحة",
                Description = "الجراحة العامة والتخصصية",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Sector
            {
                Id = Guid.Parse("34c0ba68-a406-5ead-a520-5d6d5dce5837"),
                Name = "التشخيص",
                Description = "الأشعة والمختبر والتشخيص",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Sector
            {
                Id = Guid.Parse("6e35f3a8-de36-5d41-8f3d-dacbb59bbc51"),
                Name = "الأسنان",
                Description = "عيادات الأسنان المتخصصة",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Sector
            {
                Id = Guid.Parse("8030dd09-cfd6-52fc-805e-590af72269ed"),
                Name = "التجميل والليزر",
                Description = "الجلدية والتجميل والليزر",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Sector
            {
                Id = Guid.Parse("b79fd19b-c95a-51b5-9f66-7f592c80f6c9"),
                Name = "العلاج الطبيعي",
                Description = "العلاج الطبيعي والتأهيل",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Sector
            {
                Id = Guid.Parse("f71c2946-abe7-52c3-ba07-8b1098c5ac81"),
                Name = "الإدارة",
                Description = "الإدارة والدعم",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Seed Doctors (representative sample from Excel data)
        modelBuilder.Entity<Doctor>().HasData(
            new Doctor
            {
                Id = Guid.Parse("ca83fd1e-122c-5c95-bdc5-92fd5b2bc514"),
                DisplayName = "د/ فهد القحطاني ( سكري وغدد وسمنه وباطنه )",
                Nationality = "سعودي",
                Classification = "أستشاري",
                InsuranceAcceptance = "الاطلاع عالملاحظه",
                InsuranceNotes = "",
                AvailabilityStatus = "مكتفية",
                CoordinatorName = "ساره عسيري",
                InternalExtension = "2109",
                WorkingHours = "10ِِِِAM-6PM",
                WorkingDays = "من الاحد الى الخميس",
                AgeGroup = "فوق 13 سنه",
                ConsultationFee = 850.0m,
                Services = "بكج السمنه 1100 ريال - بكج السكري 1200 ريال - بكج الغده بدون اشاعه 1250 ريال بكج الغده مع الاشاعه 1400 ريال - بكج الباطنيه مع الاشاعه 1400 ريال",
                ClinicMechanism = "توزيع الارقام قبل العياده بيوم من قسم خدمة العملاء",
                Notes = "بالنسبه للتأمين ما يقبل الدكتور الا حاله وحده تكون يوم الاحد الساعه 12Am فقط // التوضيح للمرضى بدفع مبلغ الكشفيه 850 ريال في حال بيحلل في مستشفى اخر وتكون التحاليل تاريخها جديد ما تتعدا شهر ايضا التوضيح للمريض انه اذا حلل لدينا بيدفع فقط مبلغ التحاليل المطلوبه منه من غير الكشفيه سواء مراجعه او اول زياره اذا حلل خارج العياده بيدفع الكشف اذا حلل في العياده مايدفع الكشف عدد الحجوزات بالسستم 100 حاله فقط ممنوع اضافة اي حاله زياده عليها و ممنوع اظافة اي حاله بعد تاكيد العياده يتم التاكيد قبل المواعيد ب 4 ايام اي موعد ينظاف بينلغي",
                IsActive = true,
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                SpecialtyId = Guid.Parse("5e2dd555-d7c9-52a7-ba1e-4f366ec82ce9"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("91e46c5b-12d0-57b5-8ca3-96af8150ae5e"),
                DisplayName = "د/ محمد الموسى ( غدد - سكري اطفال )",
                Nationality = "سعودي",
                Classification = "أستشاري",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "إجازة",
                CoordinatorName = "ساره عسيري",
                InternalExtension = "2109",
                WorkingHours = "1pm - 3:45pm",
                WorkingDays = "يوم الثلاثاء",
                AgeGroup = "من 18 سنه واقل",
                ConsultationFee = 300.0m,
                Services = "يستقبل حالات السكري والغدد والسمنه والنمو وتأخير البلوغ و البلوغ المبكر",
                ClinicMechanism = "توزيع الارقام قبل العياده بيوم من قسم خدمة العملاء",
                Notes = "الحجوزات 20 حاله فقطططططططط",
                IsActive = true,
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                SpecialtyId = Guid.Parse("75a9aa57-524d-5614-97e6-db9ff5eefd94"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("ede4a166-a2c3-55c6-b920-5a1b28fc07ab"),
                DisplayName = "د/ محمد عبدالباسط ( باطنيه )",
                Nationality = "مصري",
                Classification = "أخصائي",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "منيره الشهري",
                InternalExtension = "2109",
                WorkingHours = "9AM-12PM -4PM-9PM",
                WorkingDays = "من السبت الى الخميس",
                AgeGroup = "فوق 13 سنه",
                ConsultationFee = 100.0m,
                Services = "جميع خدمات الجهاز الهضمي - والغدد والسكري والضغط",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                SpecialtyId = Guid.Parse("5e2dd555-d7c9-52a7-ba1e-4f366ec82ce9"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("fe2644c5-d968-5256-b4b1-452ee0153a05"),
                DisplayName = "د/ محمد عبدالفتاح ( عظام )",
                Nationality = "مصري",
                Classification = "أخصائي",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "منيره الشهري",
                InternalExtension = "2109",
                WorkingHours = "9AM-12PM -4PM-9PM",
                WorkingDays = "من السبت الى الخميس",
                AgeGroup = "جميع الفئات العمرية",
                ConsultationFee = 100.0m,
                Services = "",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                SpecialtyId = Guid.Parse("c60f124e-2c16-58e5-9032-ac3e17c76761"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("f7461019-ff64-5220-b040-aa220332c622"),
                DisplayName = "د/ خالد الاحمري (اضطرابات الدوخه والتوازن )",
                Nationality = "سعودي",
                Classification = "بروفيسور",
                InsuranceAcceptance = "الاطلاع عالملاحظه",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "منيره الشهري",
                InternalExtension = "2109",
                WorkingHours = "4PM -7:30PM",
                WorkingDays = "يوم الاثنين - يوم الثلاثاء",
                AgeGroup = "الكبار",
                ConsultationFee = 250.0m,
                Services = "يستقبل فقط حالات الدوخه والتوازن",
                ClinicMechanism = "بالمواعيد",
                Notes = "الحجوزات تكون متتاليه - بالنسبه للتأمين حالتين فقط على كل عياده - الدكتور ما يستقبل حالات الانف والاذن والحنجره نهائيا",
                IsActive = true,
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("9225ba40-dfb5-55b6-b698-bb562aac3d11"),
                DisplayName = "احمد علي موسى",
                Nationality = "مصري",
                Classification = "طبيب عام",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "لايوجد تنسيق",
                InternalExtension = "2051-2011",
                WorkingHours = "غير ثابت",
                WorkingDays = "غير ثابت",
                AgeGroup = "الاطفال - الكبار",
                ConsultationFee = 50.0m,
                Services = "خدمات الطوارئ",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("1e0381a6-d16c-5ede-8ff4-0c24238ef1c4"),
                DisplayName = "خالد البشير",
                Nationality = "سوداني",
                Classification = "طبيب عام",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "لايوجد تنسيق",
                InternalExtension = "2051-2011",
                WorkingHours = "غير ثابت",
                WorkingDays = "غير ثابت",
                AgeGroup = "الاطفال - الكبار",
                ConsultationFee = 50.0m,
                Services = "خدمات الطوارئ",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("4bde1d1c-49b2-598b-9991-e404bf2ffbbe"),
                DisplayName = "رشا الصالح",
                Nationality = "مصري",
                Classification = "طبيب عام",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "لايوجد تنسيق",
                InternalExtension = "2051-2011",
                WorkingHours = "غير ثابت",
                WorkingDays = "غير ثابت",
                AgeGroup = "الاطفال - الكبار",
                ConsultationFee = 50.0m,
                Services = "خدمات الطوارئ",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("a5999a33-f19b-5dc1-8f37-97a68bc9b371"),
                DisplayName = "حسن متوكل",
                Nationality = "مصري",
                Classification = "أستشاري",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "منى الشهري",
                InternalExtension = "",
                WorkingHours = "يحدد من قبل المنسقه",
                WorkingDays = "من السبت الى الخميس",
                AgeGroup = "",
                ConsultationFee = 300.0m,
                Services = "",
                ClinicMechanism = "",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("75dd6d00-fc22-542d-af65-651f6135b394"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("3c859b96-ba43-50b8-90d8-14e0d598a5b7"),
                DisplayName = "الاء عبد القادر",
                Nationality = "سوداني",
                Classification = "أستشاري",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "",
                InternalExtension = "5108",
                WorkingHours = "من ٢ م - ١٠م",
                WorkingDays = "الاحد - الاثنين - الثلاثاء - الاربعاء",
                AgeGroup = "١٨-٦٠سنه",
                ConsultationFee = 250.0m,
                Services = "الولادة القيصرية و الولادة الطبيعية ومتابعة حمل -سونار -تكيسات -مشاكل نزيف -مشاكل الدوره -موانع الحمل -لخبطة هرمونات - تنظيف بعد الاسقاطات",
                ClinicMechanism = "اضافة موعد بالنظام والاولوية بقص الفاتورة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("75dd6d00-fc22-542d-af65-651f6135b394"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("9cafd7d6-d58e-57b6-a7b5-577cf5fca4aa"),
                DisplayName = "منه ممدوح",
                Nationality = "مصري",
                Classification = "أخصائي",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "",
                InternalExtension = "",
                WorkingHours = "من 9ص-5م",
                WorkingDays = "من السبت الى الخميس",
                AgeGroup = "جميع الاعمار",
                ConsultationFee = 150.0m,
                Services = "",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("75dd6d00-fc22-542d-af65-651f6135b394"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("1fad2e3f-53e8-529d-b85a-e6af6b94b617"),
                DisplayName = "د محمد سلامه",
                Nationality = "سوري",
                Classification = "أخصائي",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "",
                InternalExtension = "",
                WorkingHours = "فترتين",
                WorkingDays = "السبت",
                AgeGroup = "جميع الاعمار",
                ConsultationFee = 100.0m,
                Services = "",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("75dd6d00-fc22-542d-af65-651f6135b394"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("97ae6301-ec62-5a69-b96d-28f5a71b42e2"),
                DisplayName = "بروفسور حسين القحطاني",
                Nationality = "سعودي",
                Classification = "بروفيسور",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "مكتفية",
                CoordinatorName = "ملاذ الشهراني",
                InternalExtension = "",
                WorkingHours = "من 9ص-11 ص",
                WorkingDays = "الاحد - الثلاثاء",
                AgeGroup = "14 سنه وفوق",
                ConsultationFee = 300.0m,
                Services = "",
                ClinicMechanism = "بالمواعيد والأرقام",
                Notes = "عيادة بديله شفت المساء عن عيادة الثلاثاء",
                IsActive = true,
                BranchId = Guid.Parse("75dd6d00-fc22-542d-af65-651f6135b394"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("f575ae08-a364-5301-a3aa-c45d8186c9a7"),
                DisplayName = "بروفيسور حسين القحطاني",
                Nationality = "سعودي",
                Classification = "بروفيسور",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "مكتفية",
                CoordinatorName = "ملاذ الشهراني",
                InternalExtension = "",
                WorkingHours = "من 5-8م",
                WorkingDays = "الاثنين - الاربعاء",
                AgeGroup = "سنه وفوق 14",
                ConsultationFee = 300.0m,
                Services = "",
                ClinicMechanism = "بالمواعيد والأرقام",
                Notes = "عيادة صباحيه بديله عن الاربعاء",
                IsActive = true,
                BranchId = Guid.Parse("75dd6d00-fc22-542d-af65-651f6135b394"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("725c9338-f872-51af-b1b0-b7759c4ccc45"),
                DisplayName = "محمد العمري 1716",
                Nationality = "سعودي",
                Classification = "طبيب عام",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد ويحتاج دعم",
                CoordinatorName = "سارة عبدالرحمن",
                InternalExtension = "",
                WorkingHours = "من 1 الى 9 م",
                WorkingDays = "سبت ثلاثاء اربعاء اخر موعد 8",
                AgeGroup = "من 14 سنه",
                ConsultationFee = 50.0m,
                Services = "علاج عصب - خلع - جميع خدمات الاسنان ماعدا التلبيسات",
                ClinicMechanism = "مواعيده بالنص ساعة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("75dd6d00-fc22-542d-af65-651f6135b394"),
                SpecialtyId = Guid.Parse("cd76245f-512e-5f8e-8025-ee81d1619b8b"),
                SectorId = Guid.Parse("6e35f3a8-de36-5d41-8f3d-dacbb59bbc51"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("1144f8c7-320d-59b0-bdd8-e83caad86476"),
                DisplayName = "سلطان القحطاني",
                Nationality = "سعودي",
                Classification = "طبيب عام",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "إجازة",
                CoordinatorName = "سارة عبدالرحمن",
                InternalExtension = "",
                WorkingHours = "من 11ص الى 9 م اخر حاله 8",
                WorkingDays = "سبت اثنين اربعاء اخر موعد 8",
                AgeGroup = "من 14سنه",
                ConsultationFee = 50.0m,
                Services = "علاج الاسنان - تنظيف - تبيض",
                ClinicMechanism = "مواعيده بالنص ساعة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("75dd6d00-fc22-542d-af65-651f6135b394"),
                SpecialtyId = Guid.Parse("cd76245f-512e-5f8e-8025-ee81d1619b8b"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("4fabe4a8-cad5-5e97-afc0-d4e4fd45c76f"),
                DisplayName = "منال الاسمري",
                Nationality = "سعودي",
                Classification = "أستشاري",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "منار الراقدي",
                InternalExtension = "3190",
                WorkingHours = "10am-2pm",
                WorkingDays = "من الاحد للاربعاء",
                AgeGroup = "20-50سنه",
                ConsultationFee = 300.0m,
                Services = "حقن مجهري تلقيح صناعي تنشيط مناظر تشخيصي منظار رحمي تشخيصي وعلاجي اشعة صبغه",
                ClinicMechanism = "بالمواعيد",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("6960ef26-da8c-5d0a-807b-ce833a006ae5"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("cb32bc20-e58d-5a98-86ad-80b8d21a6b9e"),
                DisplayName = "منال القحطاني",
                Nationality = "سعودي",
                Classification = "أستشاري",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد ويحتاج دعم",
                CoordinatorName = "منار الراقدي",
                InternalExtension = "3190",
                WorkingHours = "5pm-9pm",
                WorkingDays = "الاثنين",
                AgeGroup = "20-50 سنه",
                ConsultationFee = 300.0m,
                Services = "سلسل بول تجميل نساء موانع الحمل تكيس المبياض لخبطة الهرمونات تجميل المناطق الحساسه للنساء متابعة الحمل",
                ClinicMechanism = "بالمواعيد",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("6960ef26-da8c-5d0a-807b-ce833a006ae5"),
                SpecialtyId = Guid.Parse("3610f41b-4792-5aef-ba93-7b9c53f4d70b"),
                SectorId = Guid.Parse("8030dd09-cfd6-52fc-805e-590af72269ed"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("4328211e-607c-58ad-ba81-ea1f9dfef22e"),
                DisplayName = "ياسمين الحوري",
                Nationality = "مصري",
                Classification = "أخصائي",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد ويحتاج دعم",
                CoordinatorName = "نجود عسيري",
                InternalExtension = "3299",
                WorkingHours = "9am-12pm* 4pm-9pm",
                WorkingDays = "متواجده يوميا",
                AgeGroup = "15-60 سنه",
                ConsultationFee = 100.0m,
                Services = "متابعة حمل -سونار -تكيسات -مشاكل نزيف -مشاكل الدوره -موانع الحمل -لخبطة هرمونات -",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("6960ef26-da8c-5d0a-807b-ce833a006ae5"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("43e29c6f-278d-5547-ae37-acd0eedf7457"),
                DisplayName = "ابراهيم ابو السعود",
                Nationality = "مصري",
                Classification = "أخصائي",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "نجود",
                InternalExtension = "3299",
                WorkingHours = "9am-12pm 4pm-9pm",
                WorkingDays = "من السبت الى الخميس",
                AgeGroup = "اطفال من الولاده الى 12 سنه",
                ConsultationFee = 100.0m,
                Services = "يستقبل جميع حالات الاطفال",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("6960ef26-da8c-5d0a-807b-ce833a006ae5"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("73e61a72-b552-57e4-b255-cbd86358c7ea"),
                DisplayName = "روان عمر",
                Nationality = "سوداني",
                Classification = "طبيب عام",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "لايوجد تنسيق",
                InternalExtension = "3015",
                WorkingHours = "غير ثابت",
                WorkingDays = "غير ثابت",
                AgeGroup = "الاطفال - الكبار",
                ConsultationFee = 50.0m,
                Services = "خدمات الطوارئ",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("6960ef26-da8c-5d0a-807b-ce833a006ae5"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("b67a7b9a-e27a-5dce-871c-3dbda87f47e4"),
                DisplayName = "محمد العمري",
                Nationality = "سعودي",
                Classification = "طبيب عام",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد ويحتاج دعم",
                CoordinatorName = "منار الراقدي",
                InternalExtension = "3191",
                WorkingHours = "1Pm-9pm",
                WorkingDays = "الاثنين -الاربعاء",
                AgeGroup = "فوق ١٥ سنه",
                ConsultationFee = 50.0m,
                Services = "جميع معالجات الاسنان",
                ClinicMechanism = "بالمواعيد",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("6960ef26-da8c-5d0a-807b-ce833a006ae5"),
                SpecialtyId = Guid.Parse("cd76245f-512e-5f8e-8025-ee81d1619b8b"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("648f76bf-648e-5fee-ba19-e944f9cbfa78"),
                DisplayName = "مسفر الشهراني",
                Nationality = "سعودي",
                Classification = "أستشاري",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "نجلاء",
                InternalExtension = "3191",
                WorkingHours = "غير ثابت",
                WorkingDays = "من الاحد الى الخميس",
                AgeGroup = "20-50سنه",
                ConsultationFee = 400.0m,
                Services = "الولاده القيصريه -سونار 3D ,4D -يستقبل متابعة الحمل و الحمل الخطر و السونار التفصيلي للكشف عن العيوب الخلقية",
                ClinicMechanism = "بالمواعيد",
                Notes = "مايستقبل حالات العقم و تاخر الانجاب - مايستقبل حالات اختيار نوع جنس الجنين - lمايستقبل حالات الاسقاط المتكرره *متابعة الحمل فقط*",
                IsActive = true,
                BranchId = Guid.Parse("6960ef26-da8c-5d0a-807b-ce833a006ae5"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("716877b3-af90-589c-b825-2162a831ad24"),
                DisplayName = "سامي المصوري",
                Nationality = "سعودي",
                Classification = "أستشاري",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد ويحتاج دعم",
                CoordinatorName = "نجود عسيري",
                InternalExtension = "3299",
                WorkingHours = "حسب المواعيد",
                WorkingDays = "من السبت الى الخميس",
                AgeGroup = "19 - 50 سنه",
                ConsultationFee = 300.0m,
                Services = "حقن مجهري - تحديد الجنس - تلقيح صناعي - منظار اورام - منظار اكياس مبيض - منظار ازاله الرحم",
                ClinicMechanism = "بالمواعيد",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("6960ef26-da8c-5d0a-807b-ce833a006ae5"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("a95a76bf-6d66-58ed-9995-cabba1be77c2"),
                DisplayName = "محمد مشهور",
                Nationality = "سعودي",
                Classification = "أستشاري",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد ويحتاج دعم",
                CoordinatorName = "ليلى",
                InternalExtension = "1100",
                WorkingHours = "05pm-08pm",
                WorkingDays = "الثلاثاء فقط",
                AgeGroup = "كبار فقط",
                ConsultationFee = 250.0m,
                Services = "الربو وحساسية الصدر + امراض الباطنه + السكري + الضغط + السمنه + الغده الدرقيه + امراض الجهاز الهضمي + هشاشة العظام + التهاب النفاصل + علاج الامراض الحاده والمزمنه",
                ClinicMechanism = "بالمواعيد والأرقام",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("b87e0359-603a-5d31-a76d-cbbe18d13a8e"),
                SpecialtyId = Guid.Parse("5e2dd555-d7c9-52a7-ba1e-4f366ec82ce9"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("6df46458-8e13-597b-9ffb-f6687ef85d34"),
                DisplayName = "عبدالله شلعان",
                Nationality = "سعودي",
                Classification = "أستشاري",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "ليلى",
                InternalExtension = "1100",
                WorkingHours = "05pm-08pm",
                WorkingDays = "الأحد فقط",
                AgeGroup = "كبار فقط",
                ConsultationFee = 850.0m,
                Services = "متابعة السكري وبكجات السكر والسمنه والغدد وابر السمنه والتنحيف",
                ClinicMechanism = "بالمواعيد والأرقام",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("b87e0359-603a-5d31-a76d-cbbe18d13a8e"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("9b79b85b-4ef8-5de7-bde4-2c72fb5266eb"),
                DisplayName = "احمد العروسي",
                Nationality = "مصري",
                Classification = "أخصائي",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "إجازة",
                CoordinatorName = "ليلى",
                InternalExtension = "1100",
                WorkingHours = "9am -12pm & 04pm -09pm",
                WorkingDays = "السبت الى الخميس",
                AgeGroup = "اطفال",
                ConsultationFee = 100.0m,
                Services = "الكشف الدوري على الاطفال وعمل الفحوصات اللازمه - علاج امراض الاطفال - متابعة النمو العقلي والجسماني - استقبال الحالات الطارئه",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "العودة يوم الخميس المقبل بتاريخ 30-04-2026",
                IsActive = true,
                BranchId = Guid.Parse("b87e0359-603a-5d31-a76d-cbbe18d13a8e"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("0e23b67c-4127-5053-8b64-d13825516486"),
                DisplayName = "شيرين محمود",
                Nationality = "مصري",
                Classification = "أخصائي",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "ليلى",
                InternalExtension = "1100",
                WorkingHours = "9am -12pm & 04pm -09pm",
                WorkingDays = "السبت الى الخميس",
                AgeGroup = "كبار فقط",
                ConsultationFee = 100.0m,
                Services = "كشفيات النساء والولاده + تركيب لولب وازالته + تركيب وازالة شريحة منع الحمل + متابعة حالات الحمل+ سونار + متابعة امراض النساء",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("b87e0359-603a-5d31-a76d-cbbe18d13a8e"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("2fe48ed4-ebc6-5f4f-8c56-f2d260b5d0b7"),
                DisplayName = "رفقة البدري",
                Nationality = "سوداني",
                Classification = "طبيب عام",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "لا يوجد",
                InternalExtension = "1000",
                WorkingHours = "08am-04pm",
                WorkingDays = "الجدول غير ثابت بسبب ظروف العمل",
                AgeGroup = "كبار فقط",
                ConsultationFee = 50.0m,
                Services = "طبيب عام - معالجات عامه ومعالجات الحالات طارئه - غيار الجروح والضماد وقياس السكر والضغط والوزن والعلامات الحيويه وتحويل الحاله في حال عدم الاختصاص",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("b87e0359-603a-5d31-a76d-cbbe18d13a8e"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("ea442b99-4471-536e-82bd-931084ee1766"),
                DisplayName = "د محمد شعت",
                Nationality = "فلسطيني",
                Classification = "طبيب عام",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "لا يوجد",
                InternalExtension = "1000",
                WorkingHours = "04pm-12am",
                WorkingDays = "الجدول غير ثابت بسبب ظروف العمل",
                AgeGroup = "كبار فقط",
                ConsultationFee = 50.0m,
                Services = "طبيب عام - معالجات عامه ومعالجات الحالات طارئه - غيار الجروح والضماد وقياس السكر والضغط والوزن والعلامات الحيويه وتحويل الحاله في حال عدم الاختصاص",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("b87e0359-603a-5d31-a76d-cbbe18d13a8e"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("00633099-0be8-50b4-9c2f-865d7c0bde55"),
                DisplayName = "د ندى خضر",
                Nationality = "مصري",
                Classification = "طبيب عام",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "لم تعد متواجده",
                CoordinatorName = "لا يوجد",
                InternalExtension = "1000",
                WorkingHours = "04pm-12am",
                WorkingDays = "الجدول غير ثابت بسبب ظروف العمل",
                AgeGroup = "كبار فقط",
                ConsultationFee = 50.0m,
                Services = "طبيب عام - معالجات عامه ومعالجات الحالات طارئه - غيار الجروح والضماد وقياس السكر والضغط والوزن والعلامات الحيويه وتحويل الحاله في حال عدم الاختصاص",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("b87e0359-603a-5d31-a76d-cbbe18d13a8e"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("cbb9f702-a2ab-5537-9029-1f70efbc5e35"),
                DisplayName = "شهد عبدالوهاب",
                Nationality = "سوداني",
                Classification = "طبيب عام",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "لايوجد عيادة",
                CoordinatorName = "لا يوجد",
                InternalExtension = "1000",
                WorkingHours = "08am-04pm",
                WorkingDays = "الجدول غير ثابت بسبب ظروف العمل",
                AgeGroup = "كبار فقط",
                ConsultationFee = 50.0m,
                Services = "طبيب عام - معالجات عامه ومعالجات الحالات طارئه - غيار الجروح والضماد وقياس السكر والضغط والوزن والعلامات الحيويه وتحويل الحاله في حال عدم الاختصاص",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("b87e0359-603a-5d31-a76d-cbbe18d13a8e"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("7b8ac13e-e431-581e-9350-f242f6ad5edd"),
                DisplayName = "حليمه العيسى - ( جراحه عامه )",
                Nationality = "سعودي",
                Classification = "أستشاري",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "إجازة",
                CoordinatorName = "مرام خميس",
                InternalExtension = "",
                WorkingHours = "6 pm - 9 pm",
                WorkingDays = "الخميس",
                AgeGroup = "من عمر 14 سنة فما فوق",
                ConsultationFee = 200.0m,
                Services = "• حالات الطوارئ الجراحية • أمراض المرارة والحصوات (استئصال بالمنظار أو الجراحة المفتوحة) • التهاب الزائدة الدودية واستئصالها بالمنظار • الفتق بأنواعه (علاج جراحي بالمنظار أو المفتوح) • جراحات الثدي البسيطة • استئصال الأكياس الدهنية والجلدية • علاج البواسير والناسور الشرجي والشرخ الشرجي",
                ClinicMechanism = "حجز موعد مسبق في النظام وبالنسبة للدخول الأولوية بقص الفواتير",
                Notes = "اجازه الى اشعار اخر",
                IsActive = true,
                BranchId = Guid.Parse("b6b90fc4-3aea-5528-b9fb-bd185d09bc89"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("e11f5c92-8205-5c51-a663-04197c5a2322"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("37e3f149-13c7-5420-9057-a4f37bb8142b"),
                DisplayName = "فهد القحطاني - ( الغدد و السكري و السمنه و الباطنيه )",
                Nationality = "سعودي",
                Classification = "أستشاري",
                InsuranceAcceptance = "الاطلاع الى الملاحظة",
                InsuranceNotes = "",
                AvailabilityStatus = "تنبية",
                CoordinatorName = "مرام خميس",
                InternalExtension = "",
                WorkingHours = "10 AM",
                WorkingDays = "زيـــــــــارتة الشهريــة الشهر الجـــاي مايو يــوم الاثنيــن يوم الثلاثــــاء يوم الاربعـــاء يوم الـــخميس",
                AgeGroup = "من عمر 13 سنه فما فوق",
                ConsultationFee = 850.0m,
                Services = "بكج السمنه 1100 ريال | بكج السكري مع تخطيط القلب 1200 ريال | بكج الغده بدون اشاعه 1250 ريال بكج الغده مع الاشاعه 1400 ريال | بكج الباطنيه مع الاشاعه 1400 ريال",
                ClinicMechanism = "توزيع الارقام قبل العياده بيوم من قسم خدمة العملاء",
                Notes = "الزيـــــــــارة القادمـــــة في جيــــــــــزان شهر 5 مايو تم اضافة عيادة يوم الاثنين 04 / 05 / 2026 م يوم الثلاثــــاء 05 / 05 / 2026 م يوم الاربعـــاء 06 / 05 / 2026 م يوم الـــخميس 07 / 05 / 2026 م بالنسبه للتأمين ما يقبل الدكتور التوضيح للمرضى بدفع مبلغ الكشفيه 850 ريال في حال بيحلل في مستشفى اخر وتكون التحاليل تاريخها جديد ما تتعدا شهر ايضا التوضيح للمريض انه اذا حلل لدينا بيدفع فقط مبلغ التحاليل المطلوبه منه من غير الكشفيه سواء مراجعه او اول زياره اذا حلل خارج العياده بيدفع الكشف اذا حلل في العياده مايدفع الكشف عدد الحجوزات بالسستم 100 حاله فقط ممنوع اضافة اي حاله زياده عليها و ممنوع اضافة اي حاله بعد تاكيد العياده يتم التاكيد قبل المواعيد ب 4 ايام اي موعد ينضاف بينلغي",
                IsActive = true,
                BranchId = Guid.Parse("b6b90fc4-3aea-5528-b9fb-bd185d09bc89"),
                SpecialtyId = Guid.Parse("5e2dd555-d7c9-52a7-ba1e-4f366ec82ce9"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("eff03953-58fe-5bdc-a664-ac7b46a11abe"),
                DisplayName = "مروه عفت - ( النساء و الولادة )",
                Nationality = "مصري",
                Classification = "أخصائي",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد ويحتاج دعم",
                CoordinatorName = "مرام خميس",
                InternalExtension = "",
                WorkingHours = "4pm - 9pm",
                WorkingDays = "من السبت الى الخميس",
                AgeGroup = "جميع الفئات العمرية",
                ConsultationFee = 100.0m,
                Services = "تشخيص حالات أمراض النساء والولادة , متابعة الحمل وتنظيم الأسرة , علاج العقم",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "الفتره الصباحيه من 9ص الى 12م غير متواجده .. فترة المساء فقط من 4م الى 10م",
                IsActive = true,
                BranchId = Guid.Parse("b6b90fc4-3aea-5528-b9fb-bd185d09bc89"),
                SpecialtyId = Guid.Parse("bad82437-fe11-5e3f-b01f-9075b76577ce"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("d56be56c-63a9-5a8d-82b3-228121565110"),
                DisplayName = "نشوة حسن - ( النساء و الولادة )",
                Nationality = "مصري",
                Classification = "أخصائي",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد ويحتاج دعم",
                CoordinatorName = "مرام خميس",
                InternalExtension = "",
                WorkingHours = "9am - 12pm 5pm - 10pm",
                WorkingDays = "من السبت الى الخميس",
                AgeGroup = "جميع الفئات العمرية",
                ConsultationFee = 100.0m,
                Services = "تشخيص حالات أمراض النساء والولادة , متابعة الحمل وتنظيم الأسرة , علاج العقم",
                ClinicMechanism = "الأولوية بقص الفاتورة",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("b6b90fc4-3aea-5528-b9fb-bd185d09bc89"),
                SpecialtyId = Guid.Parse("bad82437-fe11-5e3f-b01f-9075b76577ce"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("17f8ce10-9b01-5366-bfea-10f29d649328"),
                DisplayName = "وفاء الفيفي",
                Nationality = "سعودي",
                Classification = "طبيب عام",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "رجاء حكمي",
                InternalExtension = "4290",
                WorkingHours = "من 2 م الى 10 م",
                WorkingDays = "الاربعاء والخميس الفترة الصباحية وبقيه الاسبوع مساء",
                AgeGroup = "16-50",
                ConsultationFee = 50.0m,
                Services = "حشوات علاجيه-حشوات التجميلية-علاج العصب-خراج-تنظيف-تبييض-تركيبات -خلع جراحي-خلع بسيط",
                ClinicMechanism = "بالمواعيد",
                Notes = "يومي الاربعاء والخميس الفترة الصباحية من الساعه 9 ص الى 5 م وبقية الاسبوع مساء",
                IsActive = true,
                BranchId = Guid.Parse("b6b90fc4-3aea-5528-b9fb-bd185d09bc89"),
                SpecialtyId = Guid.Parse("85bc473f-372b-568e-8bb0-cf028b27ed68"),
                SectorId = Guid.Parse("e11f5c92-8205-5c51-a663-04197c5a2322"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("73fb9574-81b0-5d53-a234-3611f8f277a5"),
                DisplayName = "زينب مدخلي",
                Nationality = "سعودي",
                Classification = "طبيب عام",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "رجاء حكمي",
                InternalExtension = "4290",
                WorkingHours = "من 2 م الى 10 م",
                WorkingDays = "الاثنين والثلاثاء الفترة الصباحية وبقية الاسبوع مساءً",
                AgeGroup = "16-50",
                ConsultationFee = 50.0m,
                Services = "حشوات علاجيه-حشوات التجميلية-علاج العصب-خراج-تنظيف-تبييض-تركيبات -خلع جراحي-خلع بسيط",
                ClinicMechanism = "بالمواعيد",
                Notes = "يومي الاثنين والثلاثاء الفترة الصباحية من 9 ص الى 5 م وبقية الاسبوع مساء",
                IsActive = true,
                BranchId = Guid.Parse("b6b90fc4-3aea-5528-b9fb-bd185d09bc89"),
                SpecialtyId = Guid.Parse("85bc473f-372b-568e-8bb0-cf028b27ed68"),
                SectorId = Guid.Parse("e11f5c92-8205-5c51-a663-04197c5a2322"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("b9bd2319-e81f-5d30-8d17-b201331b6211"),
                DisplayName = "ايهاب الأمير-(جراحه عامه وجراحه الغدد الصماء و جراحة الاورام )",
                Nationality = "سعودي",
                Classification = "أستشاري",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد ويحتاج دعم",
                CoordinatorName = "مرام خميس",
                InternalExtension = "",
                WorkingHours = "5:30 pm - 9 pm",
                WorkingDays = "الاربعاء",
                AgeGroup = "جميع الفئات العمريه",
                ConsultationFee = 200.0m,
                Services = "معاينة حالات أورام الغدد الصماء وأورام وأمراض الغدة الدرقية حالات أورام الغدد جارة الدرقية حالات أورام الفم واللسان حالات أورام الغدد اللعابية حالات أورام الرقبة",
                ClinicMechanism = "حجز موعد مسبق في النظام وبالنسبة للدخول الأولوية بقص الفواتير",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("b6b90fc4-3aea-5528-b9fb-bd185d09bc89"),
                SpecialtyId = Guid.Parse("85bc473f-372b-568e-8bb0-cf028b27ed68"),
                SectorId = Guid.Parse("e11f5c92-8205-5c51-a663-04197c5a2322"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("99be041d-286e-59f0-af8d-b2f194694f64"),
                DisplayName = "منصور مباركي - (الأطفال )",
                Nationality = "سعودي",
                Classification = "أستشاري",
                InsuranceAcceptance = "يقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد ويحتاج دعم",
                CoordinatorName = "مرام خميس",
                InternalExtension = "",
                WorkingHours = "5:30 pm - 7:50 pm",
                WorkingDays = "الاحد",
                AgeGroup = "من 0-14 سنه",
                ConsultationFee = 200.0m,
                Services = "تشخيص حالات الاطفال الامراض الصدريه والتنفسيه العلويه والسفليه امراض الربو الإلتهاب الصدريه لدى الاطفال وحالات ضعف النمر لدى الاطفال الكشف المبكر للأمراض السلوكية لدى الأطفال",
                ClinicMechanism = "حجز موعد مسبق في النظام وبالنسبة للدخول الأولوية بقص الفواتير",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("b6b90fc4-3aea-5528-b9fb-bd185d09bc89"),
                SpecialtyId = Guid.Parse("970a8508-ae8b-5af6-9f01-be501358496d"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("480e3ff9-7ca2-579d-b47c-947c515562b5"),
                DisplayName = "سلطان الشهري",
                Nationality = "سعودي",
                Classification = "أستشاري",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "",
                InternalExtension = "573889811",
                WorkingHours = "1م - 4م",
                WorkingDays = "السبت - الأحد - الثلاثاء - الأربعاء",
                AgeGroup = "جميع الاعمار",
                ConsultationFee = 300.0m,
                Services = "الجلطات الدماغية، اصابات الحبل الشوكي ، اصابات الدماغ والتصلب المتعدد، الشلل التشنجي",
                ClinicMechanism = "بالمواعيد",
                Notes = "حجز المرضى الجدد على عيادة طبيب التأهيل، ويتم تحويله للأقسام الأخرى بتوجيهه والحجز لذلك",
                IsActive = true,
                BranchId = Guid.Parse("031f6a45-7d78-54b2-8bf0-b80c7a8ea289"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("75aea90e-4c8d-54c6-af0e-3f35802ff637"),
                DisplayName = "علية القرني",
                Nationality = "سعودي",
                Classification = "أستشاري",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "تنبية",
                CoordinatorName = "",
                InternalExtension = "573889811",
                WorkingHours = "5م-9م",
                WorkingDays = "السبت - الأربعاء",
                AgeGroup = "جميع الاعمار",
                ConsultationFee = 300.0m,
                Services = "استشارية التوحد",
                ClinicMechanism = "بالمواعيد",
                Notes = "مباشرتها في 1-مايو",
                IsActive = true,
                BranchId = Guid.Parse("031f6a45-7d78-54b2-8bf0-b80c7a8ea289"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("a8d3920d-4ccd-5814-88af-0cf357da2d6f"),
                DisplayName = "أثير العلي",
                Nationality = "سعودي",
                Classification = "أخصائي",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "إنتقل لفرع أخر",
                CoordinatorName = "",
                InternalExtension = "6140",
                WorkingHours = "",
                WorkingDays = "",
                AgeGroup = "",
                ConsultationFee = null,
                Services = "",
                ClinicMechanism = "بالمواعيد",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("031f6a45-7d78-54b2-8bf0-b80c7a8ea289"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("020dc213-dcab-5a1e-a74a-8a457f2dce20"),
                DisplayName = "رحاب عسيري",
                Nationality = "سعودي",
                Classification = "أخصائي",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "",
                InternalExtension = "573889811",
                WorkingHours = "3م-7م",
                WorkingDays = "يومياً عدا الجمعة",
                AgeGroup = "جميع الاعمار",
                ConsultationFee = 100.0m,
                Services = "علاج مشاكل العظام والعضلات وتأهيل ما بعد العمليات والجلطات.",
                ClinicMechanism = "بالمواعيد",
                Notes = "حجز المرضى الجدد للعيادة مباشرة وتنسيق مواعيد المتابعة من الاستقبال، التأكد من وجود تقرير أو تحويل طبي لا يزيد عن 3 شهور يمكن حجز موعد كشف لدى د.سلطان في حال الحاجة للكشف والتقييم لجميع خدمات التأهيل العصبي",
                IsActive = true,
                BranchId = Guid.Parse("031f6a45-7d78-54b2-8bf0-b80c7a8ea289"),
                SpecialtyId = Guid.Parse("c60f124e-2c16-58e5-9032-ac3e17c76761"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("4f1a6595-2df0-51b5-b94b-de1c661e8cb8"),
                DisplayName = "هاجر القحطاني",
                Nationality = "سعودي",
                Classification = "أخصائي",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "",
                InternalExtension = "573889811",
                WorkingHours = "1م-4م",
                WorkingDays = "يومياً عدا الجمعة",
                AgeGroup = "جميع الاعمار",
                ConsultationFee = 100.0m,
                Services = "علاج مشاكل العظام والعضلات وتأهيل ما بعد العمليات والجلطات.",
                ClinicMechanism = "بالمواعيد",
                Notes = "حجز المرضى الجدد للعيادة مباشرة وتنسيق مواعيد المتابعة من الاستقبال، التأكد من وجود تقرير أو تحويل طبي لا يزيد عن 3 شهور يمكن حجز موعد كشف لدى د.سلطان في حال الحاجة للكشف والتقييم لجميع خدمات التأهيل العصبي",
                IsActive = true,
                BranchId = Guid.Parse("031f6a45-7d78-54b2-8bf0-b80c7a8ea289"),
                SpecialtyId = Guid.Parse("c60f124e-2c16-58e5-9032-ac3e17c76761"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("0374da6c-06f5-5509-94c9-e459e525192f"),
                DisplayName = "مساعد الشهراني",
                Nationality = "سعودي",
                Classification = "أخصائي",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "",
                InternalExtension = "573889811",
                WorkingHours = "5م-9م",
                WorkingDays = "يومياً عدا الجمعة",
                AgeGroup = "جميع الاعمار",
                ConsultationFee = 100.0m,
                Services = "علاج مشاكل العظام والعضلات وتأهيل ما بعد العمليات والجلطات.",
                ClinicMechanism = "بالمواعيد",
                Notes = "حجز المرضى الجدد للعيادة مباشرة وتنسيق مواعيد المتابعة من الاستقبال، التأكد من وجود تقرير أو تحويل طبي لا يزيد عن 3 شهور يمكن حجز موعد كشف لدى د.سلطان في حال الحاجة للكشف والتقييم لجميع خدمات التأهيل العصبي",
                IsActive = true,
                BranchId = Guid.Parse("031f6a45-7d78-54b2-8bf0-b80c7a8ea289"),
                SpecialtyId = Guid.Parse("c60f124e-2c16-58e5-9032-ac3e17c76761"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("8722eb89-d6ec-507e-994c-15d4ca390adb"),
                DisplayName = "عمر عصام",
                Nationality = "مصري",
                Classification = "أخصائي",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "لايوجد عيادة",
                CoordinatorName = "",
                InternalExtension = "",
                WorkingHours = "",
                WorkingDays = "",
                AgeGroup = "",
                ConsultationFee = null,
                Services = "",
                ClinicMechanism = "بالمواعيد",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("031f6a45-7d78-54b2-8bf0-b80c7a8ea289"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("1cd26be2-e3a4-5561-8e1f-937269587d4a"),
                DisplayName = "محمد العريفي",
                Nationality = "سعودي",
                Classification = "أخصائي",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "",
                InternalExtension = "3900",
                WorkingHours = "1م-9م",
                WorkingDays = "يومياً عدا الجمعة",
                AgeGroup = "رجال – جميع الأعمار أطفال – من عمر 12",
                ConsultationFee = 100.0m,
                Services = "علاج مشاكل العظام والعضلات وتأهيل ما بعد العمليات والجلطات.",
                ClinicMechanism = "بالمواعيد",
                Notes = "يتم الحجز للحالات الجديدة فقط، ومواعيد المتابعة عن طريق القسم",
                IsActive = true,
                BranchId = Guid.Parse("031f6a45-7d78-54b2-8bf0-b80c7a8ea289"),
                SpecialtyId = Guid.Parse("c60f124e-2c16-58e5-9032-ac3e17c76761"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("21a33dba-3db5-566d-945f-298ac48187c0"),
                DisplayName = "سلطان الشهري",
                Nationality = "سعودي",
                Classification = "أستشاري",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "",
                InternalExtension = "",
                WorkingHours = "5م - 9م",
                WorkingDays = "السبت - الأحد - الثلاثاء - الأربعاء",
                AgeGroup = "جميع الاعمار",
                ConsultationFee = 300.0m,
                Services = "الجلطات الدماغية، اصابات الحبل الشوكي ، اصابات الدماغ والتصلب المتعدد، الشلل التشنجي",
                ClinicMechanism = "بالمواعيد",
                Notes = "حجز المرضى الجدد على عيادة طبيب التأهيل، ويتم تحويله للأقسام الأخرى بتوجيهه والحجز لذلك",
                IsActive = true,
                BranchId = Guid.Parse("f3c98415-4f36-5a99-a16d-d4d9da20d782"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("8d4cdd28-e571-53ba-92a6-01bb9e73551e"),
                DisplayName = "شيماء أمين",
                Nationality = "مصري",
                Classification = "أستشاري",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "",
                InternalExtension = "",
                WorkingHours = "4م-8م",
                WorkingDays = "الثلاثاء",
                AgeGroup = "جميع الاعمار",
                ConsultationFee = null,
                Services = "",
                ClinicMechanism = "بالمواعيد",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("f3c98415-4f36-5a99-a16d-d4d9da20d782"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("90de55be-89ab-5e5b-ab2b-6dfdfae685c8"),
                DisplayName = "أثير العلي",
                Nationality = "سعودي",
                Classification = "أخصائي",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "لايوجد عيادة",
                CoordinatorName = "",
                InternalExtension = "",
                WorkingHours = "",
                WorkingDays = "",
                AgeGroup = "",
                ConsultationFee = null,
                Services = "",
                ClinicMechanism = "بالمواعيد",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("f3c98415-4f36-5a99-a16d-d4d9da20d782"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("8df80e50-c583-568e-9071-85f0ff4cfdeb"),
                DisplayName = "رحاب عسيري",
                Nationality = "سعودي",
                Classification = "أخصائي",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "",
                InternalExtension = "",
                WorkingHours = "3م-7م",
                WorkingDays = "يومياً عدا الجمعة",
                AgeGroup = "جميع الاعمار",
                ConsultationFee = null,
                Services = "علاج مشاكل العظام والعضلات وتأهيل ما بعد الجلطات.",
                ClinicMechanism = "بالمواعيد",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("f3c98415-4f36-5a99-a16d-d4d9da20d782"),
                SpecialtyId = Guid.Parse("c60f124e-2c16-58e5-9032-ac3e17c76761"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("16b5603a-1b0b-546a-981b-41b6caa4aea4"),
                DisplayName = "هاجر القحطاني",
                Nationality = "سعودي",
                Classification = "أخصائي",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "",
                InternalExtension = "",
                WorkingHours = "1م-4م",
                WorkingDays = "يومياً عدا الجمعة",
                AgeGroup = "جميع الاعمار",
                ConsultationFee = null,
                Services = "علاج مشاكل العظام والعضلات وتأهيل ما بعد الجلطات.",
                ClinicMechanism = "بالمواعيد",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("f3c98415-4f36-5a99-a16d-d4d9da20d782"),
                SpecialtyId = Guid.Parse("c60f124e-2c16-58e5-9032-ac3e17c76761"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("0b364f3e-d9fc-5040-9332-9ec0d19ea922"),
                DisplayName = "مساعد الشهراني",
                Nationality = "سعودي",
                Classification = "أخصائي",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "",
                InternalExtension = "",
                WorkingHours = "5م-9م",
                WorkingDays = "يومياً عدا الجمعة",
                AgeGroup = "جميع الاعمار",
                ConsultationFee = null,
                Services = "علاج مشاكل العظام والعضلات وتأهيل ما بعد الجلطات.",
                ClinicMechanism = "بالمواعيد",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("f3c98415-4f36-5a99-a16d-d4d9da20d782"),
                SpecialtyId = Guid.Parse("c60f124e-2c16-58e5-9032-ac3e17c76761"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("8bfbb342-bf7c-5444-b92a-3270fe73357d"),
                DisplayName = "عمر عصام",
                Nationality = "مصري",
                Classification = "أخصائي",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "لايوجد عيادة",
                CoordinatorName = "",
                InternalExtension = "",
                WorkingHours = "",
                WorkingDays = "",
                AgeGroup = "",
                ConsultationFee = null,
                Services = "",
                ClinicMechanism = "بالمواعيد",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("f3c98415-4f36-5a99-a16d-d4d9da20d782"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Doctor
            {
                Id = Guid.Parse("23055957-0807-5be8-a2a5-c7077fa383f2"),
                DisplayName = "موسى الغامدي",
                Nationality = "سعودي",
                Classification = "أخصائي",
                InsuranceAcceptance = "لايقبل تأمين",
                InsuranceNotes = "",
                AvailabilityStatus = "متواجد",
                CoordinatorName = "",
                InternalExtension = "",
                WorkingHours = "1م - 4م",
                WorkingDays = "السبت - الأحد - الاثنين - الثلاثاء",
                AgeGroup = "جميع الاعمار",
                ConsultationFee = null,
                Services = "تأهيل القدرات الإدراكية والمعرفية، تنمية المهارات الحركية الدقيقة، تعزيز الاستقلالية في انشطة الحياة اليومية",
                ClinicMechanism = "بالمواعيد",
                Notes = "",
                IsActive = true,
                BranchId = Guid.Parse("f3c98415-4f36-5a99-a16d-d4d9da20d782"),
                SpecialtyId = Guid.Parse("d6db5dfe-e994-5581-b8dd-2ac69f87a5ab"),
                SectorId = Guid.Parse("ce5c55ed-5a1b-54b1-9895-8e04b1b8c9e0"),
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Seed FAQ
        modelBuilder.Entity<FaqItem>().HasData(
            new FaqItem
            {
                Id = Guid.Parse("3eedc275-eb6d-53fe-8a8c-6d59a688ab79"),
                Question = "ما انواع السونار ؟ واسعارهم ؟",
                Answer = "سونا عادي ومهبلي وثلاثي الابعاد الاسعار تتحدد من الاستقبال",
                Category = "عام",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new FaqItem
            {
                Id = Guid.Parse("ed237b60-bf27-5d6b-8f07-6a07840b841d"),
                Question = "هل الاطباء يمكن يعملون خصومات للمراجعين ؟",
                Answer = "عن طريق الطبيب الصلاحيه ولكن لا يتم ذكرها للمراجع",
                Category = "عام",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new FaqItem
            {
                Id = Guid.Parse("8668a2a1-20bf-5bcd-9f79-b42a62170647"),
                Question = "كم ضريبة الاجنبي ؟",
                Answer = "15%",
                Category = "عام",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new FaqItem
            {
                Id = Guid.Parse("d3c730d3-d66b-5eee-aa4a-7d2d5c7df181"),
                Question = "هل يوجد اقساط بجميع الفروع ؟",
                Answer = "اي نعم  تابي وتمارا وتسهيل - ماعدى المستشفى تابي وتسهيل فقط",
                Category = "عام",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new FaqItem
            {
                Id = Guid.Parse("faf9d682-66b5-5977-814c-87592be28534"),
                Question = "ما انواع اجهزه الموجوده لتكسير الدهون ما اسعارها ؟",
                Answer = "جهاز الانكيرف  وكمان عمليات التجميل الجراحيه",
                Category = "عام",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new FaqItem
            {
                Id = Guid.Parse("0ce67849-fa57-5742-ae81-478241e931f0"),
                Question = "سعر المنطقة ليزر جنتل برو والكلاريتي ؟",
                Answer = "جنتل برو 150 الكلاريتي 200",
                Category = "عام",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new FaqItem
            {
                Id = Guid.Parse("9751ab93-3fe7-5e67-a2f9-5f4314e9d182"),
                Question = "كم سعر الأشعة التفصليه ؟",
                Answer = "يبدا من400 وتحدد من قبل الدكتور",
                Category = "عام",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new FaqItem
            {
                Id = Guid.Parse("eeb4613d-1800-57aa-8dcc-2f7a3b7b51b2"),
                Question = "هل يوجد الكي البارد في فرع الخميس؟",
                Answer = "لا يوجد كي بارد يوجد ليزر فقط لفطريات السمكه في القدم",
                Category = "عام",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new FaqItem
            {
                Id = Guid.Parse("9d9f9726-b52d-5577-b1c0-23e00794d827"),
                Question = "انواع زراعة الأسنان ؟",
                Answer = "نوع الزراعة سترومن سويسرية",
                Category = "عام",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new FaqItem
            {
                Id = Guid.Parse("10b2dca1-a51e-5257-9834-4b155d386ae4"),
                Question = "هل رسوم الخياطة تشمل فك الخياطة بعدين ؟",
                Answer = "لا رسوم الخياطة تختلف عن رسوم فك الخياطة",
                Category = "عام",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new FaqItem
            {
                Id = Guid.Parse("112fd842-0dea-5cee-89e1-1bbd2c288ea4"),
                Question = "هل توجد تطعيمات للاطفال ؟",
                Answer = "لايوجد تطعيمات في جميع الفروع",
                Category = "عام",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new FaqItem
            {
                Id = Guid.Parse("bd407754-4482-560a-bd08-8968379fb022"),
                Question = "سعر تركيب شريحة منع الحمل ؟",
                Answer = "1200.0",
                Category = "عام",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new FaqItem
            {
                Id = Guid.Parse("bf94e6f1-06b3-5d29-8056-c1e371a97591"),
                Question = "سعر فك شريحة الحمل ؟",
                Answer = "500.0",
                Category = "عام",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new FaqItem
            {
                Id = Guid.Parse("2c3e6213-3f0e-53be-80dc-f9e685d7c4fd"),
                Question = "الأشعة (سونارالعادي)",
                Answer = "250 ريال تقريبا",
                Category = "عام",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new FaqItem
            {
                Id = Guid.Parse("0709cc00-dc37-57be-8025-10c20431cc80"),
                Question = "الأشعة (التفصيلي) 4D",
                Answer = "350 ريال",
                Category = "عام",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Seed Users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Email = "admin@tadawi.med",
                PasswordHash = "$2a$11$xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                FirstName = "System",
                LastName = "Admin",
                Role = Domain.Enums.UserRole.Admin,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
