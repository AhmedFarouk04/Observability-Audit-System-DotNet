namespace Application.DTOs;

public class MetricsErrorsDto
{
    public long ErrorCount { get; init; }

    public double ErrorRate { get; init; }

    public long TotalRequests { get; init; }

    public int WindowMinutes { get; init; }
}
