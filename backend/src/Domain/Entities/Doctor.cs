namespace Domain.Entities;

public class Doctor
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public List<string> Languages { get; set; } = new();
    public string? Qualifications { get; set; }
    public string? Biography { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public Guid SpecialtyId { get; set; }
    public Specialty Specialty { get; set; } = null!;

    public Guid SectorId { get; set; }
    public Sector Sector { get; set; } = null!;
}
