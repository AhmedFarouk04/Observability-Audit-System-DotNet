using Serilog;
using Serilog.Configuration;
using Serilog.Formatting.Json;

namespace Infrastructure.Logging.Sinks;

public static class ConsoleSinkConfiguration
{
    public static LoggerConfiguration AddJsonConsoleSink(this LoggerSinkConfiguration sinkConfiguration)
    {
        return sinkConfiguration.Console(new JsonFormatter());
    }
}
