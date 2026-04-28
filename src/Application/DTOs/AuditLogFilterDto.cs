using Domain.Entities;

namespace Application.DTOs;

public class AuditLogFilterDto
{
    public string? UserId { get; init; }

    public string? Action { get; init; }

    public AuditLogStatus? Status { get; init; }

    public DateTime? From { get; init; }

    public DateTime? To { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}
