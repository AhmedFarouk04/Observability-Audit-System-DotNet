using Domain.Entities;

namespace Domain.Repositories;

public interface IMetricRepository
{
    Task AddSnapshotAsync(MetricSnapshot snapshot, CancellationToken cancellationToken = default);

    Task<MetricSnapshot?> GetLatestAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<MetricSnapshot>> GetRangeAsync(
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default);
}
