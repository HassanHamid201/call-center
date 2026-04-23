namespace Api.DTOs;

public record AuditLogDto(
    Guid Id,
    string EntityType,
    string EntityId,
    string Action,
    string? OldValues,
    string? NewValues,
    string? UserEmail,
    string? IpAddress,
    DateTime CreatedAt
);

public record AuditQueryRequest(
    string? EntityType,
    string? EntityId,
    string? Action,
    DateTime? FromDate,
    DateTime? ToDate,
    int Page = 1,
    int PageSize = 50
);
