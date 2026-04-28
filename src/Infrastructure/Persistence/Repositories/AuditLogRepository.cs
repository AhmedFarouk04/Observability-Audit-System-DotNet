using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Infrastructure.Persistence.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AuditDbContext _context;

    public AuditLogRepository(AuditDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PagedResult<AuditLog>> GetPagedAsync(AuditLogFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.UserId))
        {
            query = query.Where(x => x.UserId == filter.UserId);
        }

        if (!string.IsNullOrWhiteSpace(filter.Action))
        {
            query = query.Where(x => x.Action.Contains(filter.Action));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        if (filter.From.HasValue)
        {
            query = query.Where(x => x.Timestamp >= filter.From.Value);
        }

        if (filter.To.HasValue)
        {
            query = query.Where(x => x.Timestamp <= filter.To.Value);
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.Timestamp)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AuditLog>
        {
            Items = items,
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<IReadOnlyCollection<AuditLog>> GetByUserIdAsync(
        string userId,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Timestamp)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> PurgeOlderThanAsync(DateTime cutoffDateUtc, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.AuditLogs
                .Where(x => x.Timestamp < cutoffDateUtc)
                .ExecuteDeleteAsync(cancellationToken);
        }
        catch (InvalidOperationException)
        {
            // Fallback for providers that do not support ExecuteDelete (e.g., InMemory in tests).
            var rows = await _context.AuditLogs
                .Where(x => x.Timestamp < cutoffDateUtc)
                .ToListAsync(cancellationToken);

            _context.AuditLogs.RemoveRange(rows);
            await _context.SaveChangesAsync(cancellationToken);
            return rows.Count;
        }
    }

    public async Task<int> GetCountByActionAsync(
        string action,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .Where(x => x.Action == action && x.Timestamp >= fromUtc && x.Timestamp <= toUtc)
            .CountAsync(cancellationToken);
    }
}
