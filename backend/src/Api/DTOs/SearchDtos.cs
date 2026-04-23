namespace Api.DTOs;

public record SearchRequest(
    string? Query,
    string? City,
    Guid? SpecialtyId,
    Guid? SectorId,
    Guid? BranchId,
    string? Language,
    int Page = 1,
    int PageSize = 20
);

public record SearchResultDto(
    Guid Id,
    string Type, // "Doctor" or "Branch"
    string Title,
    string? Subtitle,
    string? Description,
    string? Url
);

public record PagedResult<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
