using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Monitoring.HealthChecks;

public static class HealthCheckConfiguration
{
    public static IHealthChecksBuilder AddProjectHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is missing.");

        return services
            .AddHealthChecks()
            .AddSqlServer(connectionString, name: "sqlserver", tags: ["database"]);
    }
}
