using Application.DTOs;
using Application.Interfaces;
using MediatR;
using SharedKernel.Results;

namespace Application.Queries.GetMetricsSummary;

public sealed class GetMetricsSummaryHandler : IRequestHandler<GetMetricsSummaryQuery, Result<MetricsSummaryDto>>
{
    private readonly IMetricsService _metricsService;

    public GetMetricsSummaryHandler(IMetricsService metricsService)
    {
        _metricsService = metricsService;
    }

    public async Task<Result<MetricsSummaryDto>> Handle(GetMetricsSummaryQuery request, CancellationToken cancellationToken)
    {
        var summary = await _metricsService.GetSummaryAsync(cancellationToken);
        return Result<MetricsSummaryDto>.Success(summary);
    }
}
