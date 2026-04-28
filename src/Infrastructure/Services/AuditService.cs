using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;

namespace Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task RecordSuccessAsync(
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
        CancellationToken cancellationToken = default)
    {
        var auditLog = AuditLog.Create(
            userId,
            userEmail,
            action,
            entityType,
            correlationId,
            ipAddress,
            entityId,
            oldValues,
            newValues,
            userAgent);

        auditLog.SetDuration(durationMs);

        await _auditLogRepository.AddAsync(auditLog, cancellationToken);
    }

    public async Task RecordFailureAsync(
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
        CancellationToken cancellationToken = default)
    {
        var auditLog = AuditLog.Create(
            userId,
            userEmail,
            action,
            entityType,
            correlationId,
            ipAddress,
            entityId,
            oldValues,
            newValues,
            userAgent);

        auditLog.MarkAsFailed(errorMessage);
        auditLog.SetDuration(durationMs);

        await _auditLogRepository.AddAsync(auditLog, cancellationToken);
    }

    public async Task<IReadOnlyCollection<AuditLog>> GetByUserIdAsync(
        string userId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        return await _auditLogRepository.GetByUserIdAsync(userId, limit, cancellationToken);
    }
}
