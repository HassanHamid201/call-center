namespace Api.DTOs;

public record SpecialtyDto(
    Guid Id,
    string Name,
    string? Description,
    string? IconUrl,
    bool IsActive,
    DateTime CreatedAt,
    int DoctorCount
);

public record CreateSpecialtyRequest(string Name, string? Description, string? IconUrl);
public record UpdateSpecialtyRequest(string? Name, string? Description, string? IconUrl, bool? IsActive);
