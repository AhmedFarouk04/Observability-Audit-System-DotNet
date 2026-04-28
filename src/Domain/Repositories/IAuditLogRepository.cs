using Domain.Entities;
using SharedKernel.Results;

namespace Domain.Repositories;

public interface IAuditLogRepository
{
    Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<AuditLog>> GetPagedAsync(AuditLogFilter filter, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AuditLog>> GetByUserIdAsync(string userId, int limit = 50, CancellationToken cancellationToken = default);

    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);

    Task<int> PurgeOlderThanAsync(DateTime cutoffDateUtc, CancellationToken cancellationToken = default);

    Task<int> GetCountByActionAsync(string action, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default);
}
