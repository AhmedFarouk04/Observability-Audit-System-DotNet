using Microsoft.Extensions.Logging;

namespace Infrastructure.Logging;

public class StructuredLogger
{
    private readonly ILogger<StructuredLogger> _logger;

    public StructuredLogger(ILogger<StructuredLogger> logger)
    {
        _logger = logger;
    }

    public void Information(string messageTemplate, params object?[] args)
    {
        _logger.LogInformation(messageTemplate, args);
    }

    public void Warning(string messageTemplate, params object?[] args)
    {
        _logger.LogWarning(messageTemplate, args);
    }

    public void Error(Exception exception, string messageTemplate, params object?[] args)
    {
        _logger.LogError(exception, messageTemplate, args);
    }
}
