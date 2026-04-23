using Domain.Entities.Patients;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Contexts;

public class PatientDbContext : DbContext
{
    public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Visit> Visits => Set<Visit>();
    public DbSet<MedicalHistory> MedicalHistories => Set<MedicalHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        SeedData(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var patient1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var patient2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var patient3 = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var patient4 = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var patient5 = Guid.Parse("55555555-5555-5555-5555-555555555555");

        modelBuilder.Entity<Patient>().HasData(
            new Patient
            {
                Id = patient1,
                FullName = "أحمد محمد العلي",
                Phone = "+966-50-111-1111",
                IdentityNumber = "1234567890",
                FileNumber = "P-0001",
                DateOfBirth = new DateTime(1985, 5, 15, 0, 0, 0, DateTimeKind.Utc),
                Gender = "Male",
                Nationality = "سعودي",
                Address = "أبها، عسير",
                Email = "ahmed@example.com",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Patient
            {
                Id = patient2,
                FullName = "فاطمة عبدالله القحطاني",
                Phone = "+966-50-222-2222",
                IdentityNumber = "0987654321",
                FileNumber = "P-0002",
                DateOfBirth = new DateTime(1992, 8, 22, 0, 0, 0, DateTimeKind.Utc),
                Gender = "Female",
                Nationality = "سعودية",
                Address = "خميس مشيط، عسير",
                Email = "fatima@example.com",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Patient
            {
                Id = patient3,
                FullName = "خالد سعد الشهراني",
                Phone = "+966-50-333-3333",
                IdentityNumber = "1122334455",
                FileNumber = "P-0003",
                DateOfBirth = new DateTime(1978, 3, 10, 0, 0, 0, DateTimeKind.Utc),
                Gender = "Male",
                Nationality = "سعودي",
                Address = "جازان، جازان",
                Email = "khaled@example.com",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Patient
            {
                Id = patient4,
                FullName = "نورة ياسر الأسمري",
                Phone = "+966-50-444-4444",
                IdentityNumber = "5566778899",
                FileNumber = "P-0004",
                DateOfBirth = new DateTime(2000, 11, 5, 0, 0, 0, DateTimeKind.Utc),
                Gender = "Female",
                Nationality = "سعودية",
                Address = "أبها، عسير",
                Email = "noura@example.com",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Patient
            {
                Id = patient5,
                FullName = "عبدالرحمن صالح الحربي",
                Phone = "+966-50-555-5555",
                IdentityNumber = "6677889900",
                FileNumber = "P-0005",
                DateOfBirth = new DateTime(1995, 7, 18, 0, 0, 0, DateTimeKind.Utc),
                Gender = "Male",
                Nationality = "سعودي",
                Address = "أبها، عسير",
                Email = "abdurahman@example.com",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Appointment>().HasData(
            new Appointment
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                PatientId = patient1,
                DoctorId = Guid.Parse("ca83fd1e-122c-5c95-bdc5-92fd5b2bc514"),
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                AppointmentDate = new DateTime(2026, 4, 25, 10, 0, 0, DateTimeKind.Utc),
                Status = "Scheduled",
                Notes = "كشف أول",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Appointment
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                PatientId = patient2,
                DoctorId = Guid.Parse("91e46c5b-12d0-57b5-8ca3-96af8150ae5e"),
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                AppointmentDate = new DateTime(2026, 4, 26, 14, 0, 0, DateTimeKind.Utc),
                Status = "Scheduled",
                Notes = "متابعة غدد",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Appointment
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                PatientId = patient3,
                DoctorId = Guid.Parse("fe2644c5-d968-5256-b4b1-452ee0153a05"),
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                AppointmentDate = new DateTime(2026, 4, 24, 9, 0, 0, DateTimeKind.Utc),
                Status = "Completed",
                Notes = "كشف عظام",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Appointment
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                PatientId = patient4,
                DoctorId = Guid.Parse("f7461019-ff64-5220-b040-aa220332c622"),
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                AppointmentDate = new DateTime(2026, 4, 27, 16, 0, 0, DateTimeKind.Utc),
                Status = "Scheduled",
                Notes = "دوخة وتوازن",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Appointment
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                PatientId = patient5,
                DoctorId = Guid.Parse("ede4a166-a2c3-55c6-b920-5a1b28fc07ab"),
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                AppointmentDate = new DateTime(2026, 4, 28, 11, 0, 0, DateTimeKind.Utc),
                Status = "Cancelled",
                Notes = "تم الإلغاء من المريض",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Visit>().HasData(
            new Visit
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                PatientId = patient1,
                DoctorId = Guid.Parse("ca83fd1e-122c-5c95-bdc5-92fd5b2bc514"),
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                VisitDate = new DateTime(2026, 3, 15, 10, 0, 0, DateTimeKind.Utc),
                VisitType = "FirstVisit",
                Diagnosis = "سكري نوع 2",
                Treatment = "أدوية فموية + نظام غذائي",
                Notes = "متابعة بعد شهر",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Visit
            {
                Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                PatientId = patient2,
                DoctorId = Guid.Parse("91e46c5b-12d0-57b5-8ca3-96af8150ae5e"),
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                VisitDate = new DateTime(2026, 3, 20, 14, 0, 0, DateTimeKind.Utc),
                VisitType = "FollowUp",
                Diagnosis = "تأخر نمو",
                Treatment = "فيتامينات + جلسات تأهيل",
                Notes = "تحسن ملحوظ",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Visit
            {
                Id = Guid.Parse("11111111-2222-3333-4444-555555555555"),
                PatientId = patient3,
                DoctorId = Guid.Parse("fe2644c5-d968-5256-b4b1-452ee0153a05"),
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                VisitDate = new DateTime(2026, 2, 10, 9, 0, 0, DateTimeKind.Utc),
                VisitType = "Emergency",
                Diagnosis = "كسر في الساق",
                Treatment = "جبس + مسكنات",
                Notes = "حالة طوارئ",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Visit
            {
                Id = Guid.Parse("66666666-7777-8888-9999-000000000000"),
                PatientId = patient4,
                DoctorId = Guid.Parse("f7461019-ff64-5220-b040-aa220332c622"),
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                VisitDate = new DateTime(2026, 4, 5, 16, 0, 0, DateTimeKind.Utc),
                VisitType = "FirstVisit",
                Diagnosis = "دوخة وضعية",
                Treatment = "تمارين توازن",
                Notes = "",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Visit
            {
                Id = Guid.Parse("12121212-3434-5656-7878-909090909090"),
                PatientId = patient5,
                DoctorId = Guid.Parse("ede4a166-a2c3-55c6-b920-5a1b28fc07ab"),
                BranchId = Guid.Parse("91385c98-ebd3-5973-9d8c-116f9e486800"),
                VisitDate = new DateTime(2026, 1, 12, 11, 0, 0, DateTimeKind.Utc),
                VisitType = "FollowUp",
                Diagnosis = "ارتجاع معدي",
                Treatment = "مضادات حموضة",
                Notes = "متابعة بعد أسبوعين",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<MedicalHistory>().HasData(
            new MedicalHistory
            {
                Id = Guid.Parse("abababab-abab-abab-abab-abababababab"),
                PatientId = patient1,
                Condition = "سكري نوع 2",
                DiagnosisDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                Status = "Chronic",
                Notes = "تحت المراقبة",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new MedicalHistory
            {
                Id = Guid.Parse("cdcdcdcd-cdcd-cdcd-cdcd-cdcdcdcdcdcd"),
                PatientId = patient1,
                Condition = "ضغط دم مرتفع",
                DiagnosisDate = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                Status = "Active",
                Notes = "أدوية منتظمة",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new MedicalHistory
            {
                Id = Guid.Parse("efefefef-efef-efef-efef-efefefefefef"),
                PatientId = patient3,
                Condition = "كسر في الساق",
                DiagnosisDate = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc),
                Status = "Resolved",
                Notes = "شفاء تام",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
