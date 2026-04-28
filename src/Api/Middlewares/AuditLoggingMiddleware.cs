using Application.Interfaces;
using SharedKernel.Interfaces;
using System.Diagnostics;

namespace Api.Middlewares;

public sealed class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;

    public AuditLoggingMiddleware(
        RequestDelegate next,
        ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IAuditService auditService,
        ICurrentUserService currentUserService)
    {
        if (ShouldSkip(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.Items[CorrelationIdMiddleware.CorrelationIdHeader]?.ToString()
            ?? context.TraceIdentifier;
        var userId = currentUserService.UserId ?? "anonymous";
        var email = currentUserService.Email ?? "anonymous@local";
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var action = $"{context.Request.Method} {context.Request.Path}";

        try
        {
            await _next(context);
            stopwatch.Stop();

            if (context.Response.StatusCode >= StatusCodes.Status500InternalServerError)
            {
                await TryRecordFailureAsync(
                    auditService,
                    userId,
                    email,
                    action,
                    correlationId,
                    ipAddress,
                    $"Request failed with HTTP {context.Response.StatusCode}.",
                    userAgent,
                    stopwatch.ElapsedMilliseconds,
                    context.RequestAborted);

                return;
            }

            await TryRecordSuccessAsync(
                auditService,
                userId,
                email,
                action,
                correlationId,
                ipAddress,
                userAgent,
                stopwatch.ElapsedMilliseconds,
                context.RequestAborted);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            await TryRecordFailureAsync(
                auditService,
                userId,
                email,
                action,
                correlationId,
                ipAddress,
                ex.Message,
                userAgent,
                stopwatch.ElapsedMilliseconds,
                context.RequestAborted);

            throw;
        }
    }

    private async Task TryRecordSuccessAsync(
        IAuditService auditService,
        string userId,
        string email,
        string action,
        string correlationId,
        string ipAddress,
        string userAgent,
        long durationMs,
        CancellationToken cancellationToken)
    {
        try
        {
            await auditService.RecordSuccessAsync(
                userId,
                email,
                action,
                "HttpRequest",
                correlationId,
                ipAddress,
                userAgent: userAgent,
                durationMs: durationMs,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to persist audit success log for {Action}.", action);
        }
    }

    private async Task TryRecordFailureAsync(
        IAuditService auditService,
        string userId,
        string email,
        string action,
        string correlationId,
        string ipAddress,
        string errorMessage,
        string userAgent,
        long durationMs,
        CancellationToken cancellationToken)
    {
        try
        {
            await auditService.RecordFailureAsync(
                userId,
                email,
                action,
                "HttpRequest",
                correlationId,
                ipAddress,
                errorMessage,
                userAgent: userAgent,
                durationMs: durationMs,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to persist audit failure log for {Action}.", action);
        }
    }

    private static bool ShouldSkip(PathString path)
    {
        return path.StartsWithSegments("/metrics", StringComparison.OrdinalIgnoreCase)
            || path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase)
            || path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase);
    }
}
