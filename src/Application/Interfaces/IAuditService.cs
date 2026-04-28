using Domain.Entities;

namespace Application.Interfaces;

public interface IAuditService
{
    Task RecordSuccessAsync(
        string userId,
        string userEmail,
        string action,
        string entityType,
        string correlationId,
        string ipAddress,
        string? entityId = null,
        string? oldValues = null,
        string? newValues = null,
        string? userAgent = null,
        long durationMs = 0,
        CancellationToken cancellationToken = default);

    Task RecordFailureAsync(
        string userId,
        string userEmail,
        string action,
        string entityType,
        string correlationId,
        string ipAddress,
        string errorMessage,
        string? entityId = null,
        string? oldValues = null,
        string? newValues = null,
        string? userAgent = null,
        long durationMs = 0,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AuditLog>> GetByUserIdAsync(string userId, int limit, CancellationToken cancellationToken = default);
}
