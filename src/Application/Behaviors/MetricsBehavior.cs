using Application.Interfaces;
using MediatR;
using System.Diagnostics;

namespace Application.Behaviors;

public class MetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IMetricsService _metricsService;

    public MetricsBehavior(IMetricsService metricsService)
    {
        _metricsService = metricsService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();
            stopwatch.Stop();
            _metricsService.TrackMediatorRequest(requestName, true, stopwatch.Elapsed.TotalMilliseconds);
            return response;
        }
        catch
        {
            stopwatch.Stop();
            _metricsService.TrackMediatorRequest(requestName, false, stopwatch.Elapsed.TotalMilliseconds);
            throw;
        }
    }
}
