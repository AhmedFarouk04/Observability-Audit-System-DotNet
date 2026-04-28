using Application.DTOs;
using Application.Interfaces;
using MediatR;
using SharedKernel.Results;

namespace Application.Queries.GetMetricsErrors;

public sealed class GetMetricsErrorsHandler : IRequestHandler<GetMetricsErrorsQuery, Result<MetricsErrorsDto>>
{
    private readonly IMetricsService _metricsService;

    public GetMetricsErrorsHandler(IMetricsService metricsService)
    {
        _metricsService = metricsService;
    }

    public async Task<Result<MetricsErrorsDto>> Handle(GetMetricsErrorsQuery request, CancellationToken cancellationToken)
    {
        var errors = await _metricsService.GetErrorRateAsync(cancellationToken);
        return Result<MetricsErrorsDto>.Success(errors);
    }
}
