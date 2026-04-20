using Reveries.Infrastructure.Configuration;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Grafana.Loki;

namespace Reveries.Infrastructure.Logging;

internal static class LokiSinkExtensions
{
    public static LoggerConfiguration LokiSink(
        this LoggerSinkConfiguration sink, 
        LokiSettings settings, 
        string env, 
        LogEventLevel level)
    {
        return sink.GrafanaLoki(
            uri: settings.Uri!,
            labels:
            [
                new LokiLabel { Key = "service_name", Value = settings.AppName },
                new LokiLabel { Key = "env", Value = env.ToLower() }
            ],
            propertiesAsLabels: ["level"],
            restrictedToMinimumLevel: level,
            batchPostingLimit: settings.BatchPostingLimit,
            queueLimit: settings.QueueLimit,
            period: TimeSpan.FromSeconds(settings.PeriodSeconds),
            textFormatter: new RenderedCompactJsonFormatter()
        );
    }
}