namespace Application.DTOs;

public class MetricsSummaryDto
{
    public long TotalRequests { get; init; }

    public long TotalErrors { get; init; }

    public double ErrorRatePercent { get; init; }

    public double AverageLatencyMs { get; init; }

    public double P50LatencyMs { get; init; }

    public double P95LatencyMs { get; init; }

    public double P99LatencyMs { get; init; }

    public DateTime GeneratedAtUtc { get; init; }
}
