using Api.Authentication;
using Api.Health;
using Api.Services;
using Api.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Interfaces;
using System.Text;
using System.Threading.RateLimiting;

namespace Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddSwaggerDocumentation();
        services.AddAuditLogging();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireAuthenticatedUser().RequireRole("admin"));
        });

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            var apiPermitLimit = configuration.GetValue<int?>("RateLimiting:Api:PermitLimit") ?? 120;
            var apiWindowSeconds = configuration.GetValue<int?>("RateLimiting:Api:WindowSeconds") ?? 60;
            var authPermitLimit = configuration.GetValue<int?>("RateLimiting:Auth:PermitLimit") ?? 20;
            var authWindowSeconds = configuration.GetValue<int?>("RateLimiting:Auth:WindowSeconds") ?? 60;

            options.AddFixedWindowLimiter("api", limiter =>
            {
                limiter.PermitLimit = apiPermitLimit;
                limiter.Window = TimeSpan.FromSeconds(apiWindowSeconds);
                limiter.QueueLimit = 0;
                limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            options.AddFixedWindowLimiter("auth", limiter =>
            {
                limiter.PermitLimit = authPermitLimit;
                limiter.Window = TimeSpan.FromSeconds(authWindowSeconds);
                limiter.QueueLimit = 0;
                limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
        });

        services.AddSingleton<ITokenService, JwtTokenService>();

        services.AddHealthChecks()
            .AddCheck<LivenessCheck>("liveness", tags: ["live"])
            .AddCheck<ReadinessCheck>("readiness", tags: ["ready"])
            .AddCheck<DatabaseHealthCheck>("database", tags: ["database", "ready"]);

        return services;
    }
}
