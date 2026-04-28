using Application.DTOs;
using Application.Interfaces;
using MediatR;
using SharedKernel.Results;

namespace Application.Queries.GetMetricsLatency;

public sealed class GetMetricsLatencyHandler : IRequestHandler<GetMetricsLatencyQuery, Result<MetricsLatencyDto>>
{
    private readonly IMetricsService _metricsService;

    public GetMetricsLatencyHandler(IMetricsService metricsService)
    {
        _metricsService = metricsService;
    }

    public async Task<Result<MetricsLatencyDto>> Handle(GetMetricsLatencyQuery request, CancellationToken cancellationToken)
    {
        var latency = await _metricsService.GetLatencyPercentilesAsync(cancellationToken);
        return Result<MetricsLatencyDto>.Success(latency);
    }
}
