using Application.Behaviors;
using Application.Commands.CreateAuditLog;
using Application.Interfaces;
using Application.Validators;
using Domain.Repositories;
using FluentValidation;
using Infrastructure.Monitoring;
using Infrastructure.Monitoring.HealthChecks;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bootstrapper;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateAuditLogCommand).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(MetricsBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(CreateAuditLogValidator).Assembly);

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is missing.");

        services.AddDbContext<AuditDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddProjectHealthChecks(configuration);

        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IMetricRepository, MetricRepository>();

        services.AddScoped<IAuditService, AuditService>();
        services.AddSingleton<IMetricsService, PrometheusMetricsService>();

        return services;
    }
}
