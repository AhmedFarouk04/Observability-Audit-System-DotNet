using Application.DTOs;

namespace Application.Interfaces;

public interface IMetricsService
{
    void TrackHttpRequest(string method, string endpoint, int statusCode, double durationMs);

    void TrackMediatorRequest(string requestType, bool success, double durationMs);

    Task<MetricsSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);

    Task<MetricsErrorsDto> GetErrorRateAsync(CancellationToken cancellationToken = default);

    Task<MetricsLatencyDto> GetLatencyPercentilesAsync(CancellationToken cancellationToken = default);
}
