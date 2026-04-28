using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Api.IntegrationTests;

public class AuditEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuditEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AuditDbContext>));
                services.RemoveAll(typeof(AuditDbContext));

                services.AddDbContext<AuditDbContext>(options =>
                    options.UseInMemoryDatabase("AuditApiIntegrationDb"));
            });
        }).CreateClient();
    }

    [Fact]
    public async Task Post_AuthToken_ReturnsOkAndToken()
    {
        var payload = JsonSerializer.Serialize(new
        {
            userId = "integration-admin",
            email = "integration-admin@test.local",
            role = "admin"
        });

        var response = await _client.PostAsync(
            "/api/v1/auth/token",
            new StringContent(payload, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.TryGetProperty("accessToken", out var tokenElement).Should().BeTrue();
        tokenElement.GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Get_AuditLogs_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/audit-logs");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_AuditLogs_WithInvalidPage_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/v1/audit-logs?page=0");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_AuditLogByUnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/v1/audit-logs/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_ExportAuditLogs_ReturnsCsv()
    {
        var response = await _client.GetAsync("/api/v1/audit-logs/export");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/csv");

        var content = await response.Content.ReadAsStringAsync();
        content.Should().StartWith("Id,UserId,UserEmail");
    }

    [Fact]
    public async Task Delete_PurgeAuditLogs_WithoutToken_ReturnsUnauthorized()
    {
        var olderThan = Uri.EscapeDataString(DateTime.UtcNow.AddDays(-30).ToString("O"));
        var response = await _client.DeleteAsync($"/api/v1/audit-logs/purge?olderThan={olderThan}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_PurgeAuditLogs_WithAdminToken_ReturnsOk()
    {
        var token = await GetAdminTokenAsync();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var olderThan = Uri.EscapeDataString(DateTime.UtcNow.AddDays(-30).ToString("O"));
        var response = await _client.DeleteAsync($"/api/v1/audit-logs/purge?olderThan={olderThan}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var payload = JsonSerializer.Serialize(new
        {
            userId = "integration-admin",
            email = "integration-admin@test.local",
            role = "admin"
        });

        var response = await _client.PostAsync(
            "/api/v1/auth/token",
            new StringContent(payload, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement.GetProperty("accessToken").GetString()!;
    }
}
