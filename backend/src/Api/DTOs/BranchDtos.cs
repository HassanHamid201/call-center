namespace Api.DTOs;

public record BranchDto(
    Guid Id,
    string Name,
    string Address,
    string City,
    string? Phone,
    string? Email,
    double? Latitude,
    double? Longitude,
    bool IsActive,
    List<string> Services,
    DateTime CreatedAt,
    int DoctorCount
);

public record CreateBranchRequest(
    string Name,
    string Address,
    string City,
    string? Phone,
    string? Email,
    double? Latitude,
    double? Longitude,
    List<string>? Services
);

public record UpdateBranchRequest(
    string? Name,
    string? Address,
    string? City,
    string? Phone,
    string? Email,
    double? Latitude,
    double? Longitude,
    List<string>? Services,
    bool? IsActive
);
