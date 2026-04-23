namespace Domain.Entities.Patients;

public class MedicalHistory
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Condition { get; set; } = string.Empty;
    public DateTime? DiagnosisDate { get; set; }
    public string Status { get; set; } = "Active"; // Active | Resolved | Chronic
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
