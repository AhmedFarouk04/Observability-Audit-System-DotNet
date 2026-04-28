using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Formatting.Json;

namespace Infrastructure.Logging;

public static class SerilogConfiguration
{
    public static WebApplicationBuilder AddSerilogConfiguration(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("Application", "ObservabilityAuditSystem")
                .WriteTo.Console(new JsonFormatter())
                .WriteTo.File(
                    formatter: new JsonFormatter(),
                    path: "logs/audit-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    fileSizeLimitBytes: 100 * 1024 * 1024,
                    rollOnFileSizeLimit: true,
                    shared: true);
        });

        return builder;
    }
}
