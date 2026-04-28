using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Api.Health;

public class LivenessCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("Application is alive."));
    }
}
