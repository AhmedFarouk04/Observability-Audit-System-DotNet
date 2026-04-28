using SharedKernel.Events;

namespace Domain.Events;

public sealed class AlertTriggeredEvent : IDomainEvent
{
    public AlertTriggeredEvent(string alertType, string message)
    {
        AlertType = alertType;
        Message = message;
        OccurredOnUtc = DateTime.UtcNow;
    }

    public string AlertType { get; }

    public string Message { get; }

    public DateTime OccurredOnUtc { get; }
}
