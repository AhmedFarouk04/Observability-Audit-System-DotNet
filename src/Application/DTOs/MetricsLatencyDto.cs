namespace Application.DTOs;

public class MetricsLatencyDto
{
    public double P50Ms { get; init; }

    public double P95Ms { get; init; }

    public double P99Ms { get; init; }

    public long SampleCount { get; init; }
}
