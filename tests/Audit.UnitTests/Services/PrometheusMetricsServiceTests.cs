using FluentAssertions;
using Infrastructure.Monitoring;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Audit.UnitTests.Services;

public class PrometheusMetricsServiceTests
{
    private readonly Mock<IServiceScopeFactory> _scopeFactoryMock = new();
    private readonly Mock<ILogger<PrometheusMetricsService>> _loggerMock = new();

    [Fact]
    public async Task MetricsService_GetErrorRate_ReturnsCorrectPercentage()
    {
        var timeProvider = new MutableTimeProvider(new DateTimeOffset(2026, 01, 01, 00, 00, 00, TimeSpan.Zero));
        var service = new PrometheusMetricsService(_scopeFactoryMock.Object, _loggerMock.Object, timeProvider);

        service.TrackHttpRequest("GET", "/api/v1/audit-logs", 200, 12);
        service.TrackHttpRequest("GET", "/api/v1/audit-logs", 500, 20);

        timeProvider.Advance(TimeSpan.FromMinutes(6));

        service.TrackHttpRequest("GET", "/api/v1/metrics/errors", 200, 10);
        service.TrackHttpRequest("GET", "/api/v1/metrics/errors", 503, 30);

        var result = await service.GetErrorRateAsync();

        result.TotalRequests.Should().Be(2);
        result.ErrorCount.Should().Be(1);
        result.ErrorRate.Should().Be(50);
        result.WindowMinutes.Should().Be(5);
    }

    [Fact]
    public async Task MetricsService_GetLatency_ReturnsP95Value()
    {
        var service = new PrometheusMetricsService(_scopeFactoryMock.Object, _loggerMock.Object);

        for (var i = 1; i <= 10; i++)
        {
            service.TrackHttpRequest("GET", "/api/v1/audit-logs", 200, i * 10);
        }

        var result = await service.GetLatencyPercentilesAsync();

        result.P50Ms.Should().Be(50);
        result.P95Ms.Should().Be(100);
        result.P99Ms.Should().Be(100);
        result.SampleCount.Should().Be(10);
    }

    private sealed class MutableTimeProvider : TimeProvider
    {
        private DateTimeOffset _utcNow;

        public MutableTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow() => _utcNow;

        public void Advance(TimeSpan interval)
        {
            _utcNow = _utcNow.Add(interval);
        }
    }
}
