namespace Api.DTOs;

public record DoctorDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string? Email,
    string? Phone,
    string? Mobile,
    List<string> Languages,
    string? Qualifications,
    string? Biography,
    bool IsActive,
    DateTime CreatedAt,
    Guid BranchId,
    string BranchName,
    Guid SpecialtyId,
    string SpecialtyName,
    Guid SectorId,
    string SectorName
);

public record CreateDoctorRequest(
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string? Mobile,
    List<string>? Languages,
    string? Qualifications,
    string? Biography,
    Guid BranchId,
    Guid SpecialtyId,
    Guid SectorId
);

public record UpdateDoctorRequest(
    string? FirstName,
    string? LastName,
    string? Email,
    string? Phone,
    string? Mobile,
    List<string>? Languages,
    string? Qualifications,
    string? Biography,
    Guid? BranchId,
    Guid? SpecialtyId,
    Guid? SectorId,
    bool? IsActive
);
