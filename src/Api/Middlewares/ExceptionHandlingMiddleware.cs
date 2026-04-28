using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions;
using System.Text.Json;

namespace Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationException(context, ex);
        }
        catch (NotFoundException ex)
        {
            await HandleNotFoundException(context, ex);
        }
        catch (Exception ex)
        {
            await HandleUnhandledException(context, ex);
        }
    }

    private Task HandleValidationException(HttpContext context, ValidationException ex)
    {
        _logger.LogWarning("Validation failed for request {Path}: {@Errors}", context.Request.Path, ex.Errors);

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var payload = new ProblemDetails
        {
            Type = "ValidationError",
            Title = "Validation failed",
            Status = StatusCodes.Status400BadRequest,
            Detail = "One or more validation errors occurred.",
            Instance = context.Request.Path
        };

        payload.Extensions["errors"] = ex.Errors
            .Select(e => new { e.PropertyName, e.ErrorMessage })
            .ToArray();

        return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }

    private Task HandleNotFoundException(HttpContext context, NotFoundException ex)
    {
        _logger.LogWarning("Not found at {Path}: {Message}", context.Request.Path, ex.Message);

        context.Response.StatusCode = StatusCodes.Status404NotFound;
        context.Response.ContentType = "application/json";

        var payload = new ProblemDetails
        {
            Type = "NotFound",
            Title = "Resource not found",
            Status = StatusCodes.Status404NotFound,
            Detail = ex.Message,
            Instance = context.Request.Path
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }

    private Task HandleUnhandledException(HttpContext context, Exception ex)
    {
        var correlationId = context.Items[CorrelationIdMiddleware.CorrelationIdHeader]?.ToString();

        _logger.LogError(ex,
            "Unhandled exception at {Path}. CorrelationId: {CorrelationId}",
            context.Request.Path,
            correlationId);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var payload = new ProblemDetails
        {
            Type = "InternalServerError",
            Title = "Unexpected error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = "An unexpected error occurred.",
            Instance = context.Request.Path
        };

        payload.Extensions["correlationId"] = correlationId;

        return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
