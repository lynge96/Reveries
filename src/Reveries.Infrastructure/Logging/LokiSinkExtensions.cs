using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Grafana.Loki;

namespace Reveries.Infrastructure.Common.Logging;

public static class LokiSinkExtensions
{
    public static LoggerConfiguration LokiSink(this LoggerSinkConfiguration sink, string uri, string env, LogEventLevel level)
    {
        return sink.GrafanaLoki(
            uri: uri,
            labels:
            [
                new LokiLabel { Key = "app", Value = "reveries-api" },
                new LokiLabel { Key = "env", Value = env.ToLower() }
            ],
            restrictedToMinimumLevel: level,
            batchPostingLimit: 5000,
            queueLimit: 500000,
            period: TimeSpan.FromSeconds(5),
            textFormatter: new CompactJsonFormatter()
        );
    }
}