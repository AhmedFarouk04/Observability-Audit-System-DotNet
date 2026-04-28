using Domain.Events;
using SharedKernel.Base;

namespace Domain.Entities;

public class AuditLog : AggregateRoot
{
    private AuditLog()
    {
    }

    public string UserId { get; private set; } = string.Empty;

    public string UserEmail { get; private set; } = string.Empty;

    public string Action { get; private set; } = string.Empty;

    public string EntityType { get; private set; } = string.Empty;

    public string? EntityId { get; private set; }

    public string? OldValues { get; private set; }

    public string? NewValues { get; private set; }

    public string IpAddress { get; private set; } = string.Empty;

    public string CorrelationId { get; private set; } = string.Empty;

    public string? UserAgent { get; private set; }

    public AuditLogStatus Status { get; private set; }

    public string? ErrorMessage { get; private set; }

    public long DurationMs { get; private set; }

    public DateTime Timestamp { get; private set; }

    public static AuditLog Create(
        string userId,
        string userEmail,
        string action,
        string entityType,
        string correlationId,
        string ipAddress,
        string? entityId = null,
        string? oldValues = null,
        string? newValues = null,
        string? userAgent = null)
    {
        var log = new AuditLog
        {
            UserId = userId,
            UserEmail = userEmail,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            CorrelationId = correlationId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Status = AuditLogStatus.Success,
            Timestamp = DateTime.UtcNow
        };

        log.AddDomainEvent(new AuditLogCreatedEvent(log.Id, action, userId));
        return log;
    }

    public void MarkAsFailed(string errorMessage)
    {
        Status = AuditLogStatus.Failed;
        ErrorMessage = errorMessage;
    }

    public void SetDuration(long durationMs)
    {
        DurationMs = durationMs;
    }
}

public enum AuditLogStatus
{
    Success = 1,
    Failed = 2,
    Warning = 3
}
