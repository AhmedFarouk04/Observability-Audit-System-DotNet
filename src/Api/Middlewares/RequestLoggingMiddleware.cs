using Application.Interfaces;
using Prometheus;
using SharedKernel.Interfaces;
using System.Diagnostics;

namespace Api.Middlewares;

public class RequestLoggingMiddleware
{
    private static readonly Histogram HttpRequestDuration = Metrics.CreateHistogram(
        "http_request_duration_seconds",
        "HTTP request duration in seconds",
        new HistogramConfiguration
        {
            LabelNames = ["method", "endpoint", "status_code"]
        });

    private static readonly Counter HttpRequestTotal = Metrics.CreateCounter(
        "http_requests_total",
        "Total HTTP requests",
        new CounterConfiguration
        {
            LabelNames = ["method", "endpoint", "status_code"]
        });

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IMetricsService metricsService,
        ICurrentUserService currentUserService)
    {
        var stopwatch = Stopwatch.StartNew();
        var method = context.Request.Method;

        await _next(context);

        stopwatch.Stop();

        var endpoint = context.GetEndpoint()?.DisplayName ?? context.Request.Path.ToString();
        var statusCode = context.Response.StatusCode.ToString();
        var userId = currentUserService.UserId ?? "anonymous";

        HttpRequestDuration
            .WithLabels(method, endpoint, statusCode)
            .Observe(stopwatch.Elapsed.TotalSeconds);

        HttpRequestTotal
            .WithLabels(method, endpoint, statusCode)
            .Inc();

        metricsService.TrackHttpRequest(method, endpoint, context.Response.StatusCode, stopwatch.Elapsed.TotalMilliseconds);

        _logger.LogInformation(
            "HTTP {Method} {Endpoint} responded {StatusCode} in {ElapsedMs}ms for {UserId}",
            method,
            endpoint,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds,
            userId);
    }
}
