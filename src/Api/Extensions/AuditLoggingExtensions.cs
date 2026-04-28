using Api.Middlewares;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Extensions;

public static class AuditLoggingExtensions
{
    public static IServiceCollection AddAuditLogging(this IServiceCollection services)
    {
        return services;
    }

    public static IApplicationBuilder UseAuditLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuditLoggingMiddleware>();
    }
}
