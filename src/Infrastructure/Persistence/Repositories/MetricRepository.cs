using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MetricRepository : IMetricRepository
{
    private readonly AuditDbContext _context;

    public MetricRepository(AuditDbContext context)
    {
        _context = context;
    }

    public async Task AddSnapshotAsync(MetricSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        await _context.MetricSnapshots.AddAsync(snapshot, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<MetricSnapshot?> GetLatestAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MetricSnapshots
            .AsNoTracking()
            .OrderByDescending(x => x.Timestamp)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<MetricSnapshot>> GetRangeAsync(
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default)
    {
        return await _context.MetricSnapshots
            .AsNoTracking()
            .Where(x => x.Timestamp >= fromUtc && x.Timestamp <= toUtc)
            .OrderByDescending(x => x.Timestamp)
            .ToListAsync(cancellationToken);
    }
}
