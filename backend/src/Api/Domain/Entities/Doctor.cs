namespace Domain.Entities;

public class Doctor
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public List<string> Languages { get; set; } = new();
    public string? Qualifications { get; set; }
    public string? Biography { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Real-world operational fields from Tadawi Excel data
    public string? Nationality { get; set; }
    public string? Classification { get; set; } // طبيب عام | أخصائي | أستشاري | بروفيسور | نائب
    public string? InsuranceAcceptance { get; set; } // يقبل تأمين | لا يقبل تأمين | الاطلاع على الملاحظة
    public string? InsuranceNotes { get; set; }
    public string? AvailabilityStatus { get; set; } // متواجد | إجازة | تنبيه | معتذر عن العيادة | لا يوجد عيادة
    public string? CoordinatorName { get; set; }
    public string? InternalExtension { get; set; }
    public string? WorkingHours { get; set; }
    public string? WorkingDays { get; set; }
    public string? AgeGroup { get; set; }
    public decimal? ConsultationFee { get; set; }
    public string? Services { get; set; }
    public string? ClinicMechanism { get; set; } // الأولوية بقص الفاتورة | بالمواعيد | ...
    public string? VacationStart { get; set; }
    public string? VacationEnd { get; set; }
    public string? Notes { get; set; }

    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public Guid SpecialtyId { get; set; }
    public Specialty Specialty { get; set; } = null!;

    public Guid SectorId { get; set; }
    public Sector Sector { get; set; } = null!;
}
