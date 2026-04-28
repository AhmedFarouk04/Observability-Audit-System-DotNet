using SharedKernel.Events;

namespace Domain.Events;

public sealed class AuditLogCreatedEvent : IDomainEvent
{
    public AuditLogCreatedEvent(Guid auditLogId, string action, string userId)
    {
        AuditLogId = auditLogId;
        Action = action;
        UserId = userId;
        OccurredOnUtc = DateTime.UtcNow;
    }

    public Guid AuditLogId { get; }

    public string Action { get; }

    public string UserId { get; }

    public DateTime OccurredOnUtc { get; }
}
