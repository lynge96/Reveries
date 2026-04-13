using Serilog;
using Serilog.Events;

namespace Reveries.Api.Configuration;

public static class SerilogConfigurationExtensions
{
    public static void AddSerilogConfiguration(this WebApplicationBuilder builder)
    {
        // SelfLog.Enable(Console.WriteLine);
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    public static void ConfigureSerilogRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
            };

            // Smart log levels
            options.GetLevel = (httpContext, elapsed, ex) =>
            {
                if (ex != null || httpContext.Response.StatusCode >= 500) return LogEventLevel.Error;
                if (httpContext.Request.Path.StartsWithSegments("/health")) return LogEventLevel.Verbose;
                if (elapsed > 1000) return LogEventLevel.Warning;
                return LogEventLevel.Information;
            };
        });
    }
}