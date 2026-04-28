using Domain.Entities;

namespace Application.DTOs;

public class AuditLogDto
{
    public Guid Id { get; init; }

    public string UserId { get; init; } = string.Empty;

    public string UserEmail { get; init; } = string.Empty;

    public string Action { get; init; } = string.Empty;

    public string EntityType { get; init; } = string.Empty;

    public string? EntityId { get; init; }

    public string? OldValues { get; init; }

    public string? NewValues { get; init; }

    public string IpAddress { get; init; } = string.Empty;

    public string CorrelationId { get; init; } = string.Empty;

    public string? UserAgent { get; init; }

    public AuditLogStatus Status { get; init; }

    public string? ErrorMessage { get; init; }

    public long DurationMs { get; init; }

    public DateTime Timestamp { get; init; }
}
