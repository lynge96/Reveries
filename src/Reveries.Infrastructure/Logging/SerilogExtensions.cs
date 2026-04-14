using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Reveries.Infrastructure.Common.Logging;

public static class SerilogConfigurationExtensions
{
    public static void AddSerilog(this WebApplicationBuilder builder)
    {
        // SelfLog.Enable(Console.WriteLine);
        Log.Logger = CreateLogger(builder);
        builder.Host.UseSerilog();
    }

    public static void UseSerilogRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
                diagnosticContext.Set("TraceId", httpContext.TraceIdentifier);
                diagnosticContext.Set("CorrelationId", httpContext.Request.Headers["X-Correlation-Id"].ToString());
            };

            options.GetLevel = (httpContext, elapsed, ex) =>
            {
                if (ex != null || httpContext.Response.StatusCode >= 500) return LogEventLevel.Error;
                if (httpContext.Request.Path.StartsWithSegments("/health")) return LogEventLevel.Verbose;
                if (elapsed > 1000) return LogEventLevel.Warning;
                return LogEventLevel.Information;
            };
        });
    }

    private static Logger CreateLogger(WebApplicationBuilder builder)
    {
        var env = builder.Environment.EnvironmentName;
        var uri = builder.Configuration["Loki:Uri"];

        var level = builder.Environment.IsDevelopment()
            ? LogEventLevel.Debug
            : LogEventLevel.Information;

        return new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.LokiSink(uri!, env, level)
            .CreateLogger();
    }
}