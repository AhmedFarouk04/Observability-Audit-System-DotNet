using Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Api.Health;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly AuditDbContext _dbContext;

    public DatabaseHealthCheck(AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? HealthCheckResult.Healthy("Database check succeeded.")
                : HealthCheckResult.Unhealthy("Database check failed.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database check threw exception.", ex);
        }
    }
}
