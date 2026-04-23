namespace Api.DTOs;

public record SectorDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    int DoctorCount
);

public record CreateSectorRequest(string Name, string? Description);
public record UpdateSectorRequest(string? Name, string? Description, bool? IsActive);
