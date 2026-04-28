using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Infrastructure.Monitoring;

public class PrometheusMetricsService : IMetricsService
{
    private const int ErrorRateWindowMinutes = 5;
    private static readonly TimeSpan ErrorRateWindow = TimeSpan.FromMinutes(ErrorRateWindowMinutes);
    private const int MaxSamples = 10_000;
    private const int MaxWindowSamples = 50_000;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PrometheusMetricsService> _logger;
    private readonly TimeProvider _timeProvider;

    private long _totalRequests;
    private long _totalErrors;

    private DateTime _lastAlertAtUtc = DateTime.MinValue;

    private readonly ConcurrentQueue<double> _latencyMs = new();
    private readonly ConcurrentQueue<double> _httpLatencyMs = new();
    private readonly ConcurrentQueue<HttpRequestWindowSample> _httpRequestWindowSamples = new();

    public PrometheusMetricsService(
        IServiceScopeFactory scopeFactory,
        ILogger<PrometheusMetricsService> logger,
        TimeProvider? timeProvider = null)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public void TrackHttpRequest(string method, string endpoint, int statusCode, double durationMs)
    {
        Interlocked.Increment(ref _totalRequests);

        if (statusCode >= 500)
        {
            Interlocked.Increment(ref _totalErrors);
        }

        AddHttpWindowSample(statusCode);
        AddHttpLatencySample(durationMs);
        AddLatencySample(durationMs);
    }

    public void TrackMediatorRequest(string requestType, bool success, double durationMs)
    {
        if (!success)
        {
            Interlocked.Increment(ref _totalErrors);
        }

        AddLatencySample(durationMs);
    }

    public async Task<MetricsSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var summary = BuildSummary();

        await PersistSnapshotAsync(summary, cancellationToken);
        TryTriggerAlert(summary.ErrorRatePercent, summary.TotalRequests, summary.TotalErrors);

        return summary;
    }

    public Task<MetricsErrorsDto> GetErrorRateAsync(CancellationToken cancellationToken = default)
    {
        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
        var windowStartUtc = nowUtc - ErrorRateWindow;

        TrimOldHttpSamples(windowStartUtc);

        var requestSnapshot = _httpRequestWindowSamples.ToArray();
        var totalRequests = requestSnapshot.LongLength;
        var errorCount = requestSnapshot.LongCount(sample => sample.StatusCode >= 500);
        var errorRate = totalRequests == 0 ? 0 : Math.Round(errorCount * 100d / totalRequests, 2);

        TryTriggerAlert(errorRate, totalRequests, errorCount);

        var dto = new MetricsErrorsDto
        {
            ErrorCount = errorCount,
            ErrorRate = errorRate,
            TotalRequests = totalRequests,
            WindowMinutes = ErrorRateWindowMinutes
        };

        return Task.FromResult(dto);
    }

    public Task<MetricsLatencyDto> GetLatencyPercentilesAsync(CancellationToken cancellationToken = default)
    {
        var snapshot = _httpLatencyMs.ToArray();
        Array.Sort(snapshot);

        var dto = new MetricsLatencyDto
        {
            P50Ms = Math.Round(GetPercentile(snapshot, 0.50), 2),
            P95Ms = Math.Round(GetPercentile(snapshot, 0.95), 2),
            P99Ms = Math.Round(GetPercentile(snapshot, 0.99), 2),
            SampleCount = snapshot.LongLength
        };

        return Task.FromResult(dto);
    }

    private MetricsSummaryDto BuildSummary()
    {
        var totalRequests = Interlocked.Read(ref _totalRequests);
        var totalErrors = Interlocked.Read(ref _totalErrors);

        var snapshot = _latencyMs.ToArray();
        Array.Sort(snapshot);

        return new MetricsSummaryDto
        {
            TotalRequests = totalRequests,
            TotalErrors = totalErrors,
            ErrorRatePercent = totalRequests == 0 ? 0 : Math.Round(totalErrors * 100d / totalRequests, 2),
            AverageLatencyMs = snapshot.Length == 0 ? 0 : Math.Round(snapshot.Average(), 2),
            P50LatencyMs = Math.Round(GetPercentile(snapshot, 0.50), 2),
            P95LatencyMs = Math.Round(GetPercentile(snapshot, 0.95), 2),
            P99LatencyMs = Math.Round(GetPercentile(snapshot, 0.99), 2),
            GeneratedAtUtc = DateTime.UtcNow
        };
    }

    private async Task PersistSnapshotAsync(MetricsSummaryDto summary, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IMetricRepository>();

            var snapshot = MetricSnapshot.Create(
                summary.TotalRequests,
                summary.TotalErrors,
                summary.AverageLatencyMs,
                summary.P95LatencyMs,
                summary.P99LatencyMs);

            await repository.AddSnapshotAsync(snapshot, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to persist metrics snapshot.");
        }
    }

    private void TryTriggerAlert(double errorRatePercent, long totalRequests, long totalErrors)
    {
        var now = DateTime.UtcNow;
        var shouldTrigger = totalRequests >= 20 && errorRatePercent >= 10;

        if (!shouldTrigger || (now - _lastAlertAtUtc) < TimeSpan.FromMinutes(5))
        {
            return;
        }

        _lastAlertAtUtc = now;

        var alert = new AlertTriggeredEvent(
            "HighErrorRate",
            $"Error rate reached {errorRatePercent}% ({totalErrors}/{totalRequests}).");

        _logger.LogWarning(
            "AlertTriggeredEvent fired: {AlertType} - {Message} at {OccurredOnUtc}",
            alert.AlertType,
            alert.Message,
            alert.OccurredOnUtc);
    }

    private void AddLatencySample(double durationMs)
    {
        _latencyMs.Enqueue(durationMs);

        while (_latencyMs.Count > MaxSamples && _latencyMs.TryDequeue(out _))
        {
        }
    }

    private void AddHttpLatencySample(double durationMs)
    {
        _httpLatencyMs.Enqueue(durationMs);

        while (_httpLatencyMs.Count > MaxSamples && _httpLatencyMs.TryDequeue(out _))
        {
        }
    }

    private void AddHttpWindowSample(int statusCode)
    {
        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
        _httpRequestWindowSamples.Enqueue(new HttpRequestWindowSample(nowUtc, statusCode));

        TrimOldHttpSamples(nowUtc - ErrorRateWindow);

        while (_httpRequestWindowSamples.Count > MaxWindowSamples && _httpRequestWindowSamples.TryDequeue(out _))
        {
        }
    }

    private void TrimOldHttpSamples(DateTime windowStartUtc)
    {
        while (_httpRequestWindowSamples.TryPeek(out var sample) && sample.TimestampUtc < windowStartUtc)
        {
            _httpRequestWindowSamples.TryDequeue(out _);
        }
    }

    private static double GetPercentile(double[] sortedValues, double percentile)
    {
        if (sortedValues.Length == 0)
        {
            return 0;
        }

        var index = (int)Math.Ceiling(percentile * sortedValues.Length) - 1;
        index = Math.Clamp(index, 0, sortedValues.Length - 1);
        return sortedValues[index];
    }

    private readonly record struct HttpRequestWindowSample(DateTime TimestampUtc, int StatusCode);
}
