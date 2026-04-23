namespace Domain.Entities.Reference;

public interface IReferenceEntity
{
    Guid Id { get; set; }
    string Name { get; set; }
    bool IsActive { get; set; }
    DateTime CreatedAt { get; set; }
}
