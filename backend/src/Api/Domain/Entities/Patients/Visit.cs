namespace Domain.Entities.Patients;

public class Visit
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public Guid? BranchId { get; set; }
    public DateTime VisitDate { get; set; }
    public string VisitType { get; set; } = "FirstVisit"; // FirstVisit | FollowUp | Emergency
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
