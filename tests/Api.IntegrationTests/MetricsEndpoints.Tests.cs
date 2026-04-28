using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Text.Json;

namespace Api.IntegrationTests;

public class MetricsEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MetricsEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AuditDbContext>));
                services.RemoveAll(typeof(AuditDbContext));

                services.AddDbContext<AuditDbContext>(options =>
                    options.UseInMemoryDatabase("MetricsApiIntegrationDb"));
            });
        }).CreateClient();
    }

    [Fact]
    public async Task Get_MetricsErrors_ReturnsExpectedShape()
    {
        var response = await _client.GetAsync("/api/v1/metrics/errors");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        root.TryGetProperty("errorCount", out var errorCount).Should().BeTrue();
        root.TryGetProperty("errorRate", out var errorRate).Should().BeTrue();
        root.TryGetProperty("totalRequests", out var totalRequests).Should().BeTrue();
        root.TryGetProperty("windowMinutes", out var windowMinutes).Should().BeTrue();

        errorCount.ValueKind.Should().Be(JsonValueKind.Number);
        errorRate.ValueKind.Should().Be(JsonValueKind.Number);
        totalRequests.ValueKind.Should().Be(JsonValueKind.Number);
        windowMinutes.GetInt32().Should().Be(5);
    }

    [Fact]
    public async Task Get_MetricsLatency_ReturnsExpectedShape()
    {
        var response = await _client.GetAsync("/api/v1/metrics/latency");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        root.TryGetProperty("p50Ms", out var p50Ms).Should().BeTrue();
        root.TryGetProperty("p95Ms", out var p95Ms).Should().BeTrue();
        root.TryGetProperty("p99Ms", out var p99Ms).Should().BeTrue();
        root.TryGetProperty("sampleCount", out var sampleCount).Should().BeTrue();

        p50Ms.ValueKind.Should().Be(JsonValueKind.Number);
        p95Ms.ValueKind.Should().Be(JsonValueKind.Number);
        p99Ms.ValueKind.Should().Be(JsonValueKind.Number);
        sampleCount.ValueKind.Should().Be(JsonValueKind.Number);
    }
}
