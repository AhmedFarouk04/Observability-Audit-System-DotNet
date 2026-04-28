using SharedKernel.Base;

namespace Domain.Entities;

public class MetricSnapshot : AggregateRoot
{
    private MetricSnapshot()
    {
    }

    public DateTime Timestamp { get; private set; }

    public long TotalRequests { get; private set; }

    public long TotalErrors { get; private set; }

    public double AverageLatencyMs { get; private set; }

    public double P95LatencyMs { get; private set; }

    public double P99LatencyMs { get; private set; }

    public static MetricSnapshot Create(
        long totalRequests,
        long totalErrors,
        double averageLatencyMs,
        double p95LatencyMs,
        double p99LatencyMs)
    {
        return new MetricSnapshot
        {
            Timestamp = DateTime.UtcNow,
            TotalRequests = totalRequests,
            TotalErrors = totalErrors,
            AverageLatencyMs = averageLatencyMs,
            P95LatencyMs = p95LatencyMs,
            P99LatencyMs = p99LatencyMs
        };
    }
}
