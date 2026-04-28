using Serilog;
using Serilog.Configuration;
using Serilog.Formatting.Json;

namespace Infrastructure.Logging.Sinks;

public static class FileSinkConfiguration
{
    public static LoggerConfiguration AddAuditFileSink(this LoggerSinkConfiguration sinkConfiguration, string path)
    {
        return sinkConfiguration.File(
            formatter: new JsonFormatter(),
            path: path,
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            fileSizeLimitBytes: 100 * 1024 * 1024,
            rollOnFileSizeLimit: true,
            shared: true);
    }
}
