using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reveries.Infrastructure.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;

namespace Reveries.Infrastructure.Logging;

public static class SerilogExtensions
{
    public static void AddSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        builder.Services.AddSerilog((services, loggerConfiguration) =>
            ConfigureLogger(loggerConfiguration, services, builder.Configuration, builder.Environment));
    }

    public static void UseRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
                diagnosticContext.Set("TraceId", Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier);
                diagnosticContext.Set("CorrelationId", httpContext.Request.Headers["X-Correlation-Id"].ToString());
            };

            options.GetLevel = (httpContext, elapsed, ex) =>
            {
                if (ex != null || httpContext.Response.StatusCode >= 500)
                    return LogEventLevel.Error;

                if (httpContext.Request.Path.StartsWithSegments("/healthz"))
                    return LogEventLevel.Verbose;

                if (elapsed > 1500)
                    return LogEventLevel.Warning;

                return LogEventLevel.Information;
            };
        });
    }

    private static void ConfigureLogger(
        LoggerConfiguration loggerConfiguration,
        IServiceProvider services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        loggerConfiguration
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.WithExceptionDetails();

        if (environment.IsDevelopment())
        {
            loggerConfiguration.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}",
                theme: AnsiConsoleTheme.Code);
        }
        else
        {
            loggerConfiguration.WriteTo.Console(new RenderedCompactJsonFormatter());
        }

        var lokiSettings = configuration
            .GetSection(LokiSettings.SectionName)
            .Get<LokiSettings>() ?? new LokiSettings();

        if (!string.IsNullOrWhiteSpace(lokiSettings.Uri))
        {
            loggerConfiguration.WriteTo.LokiSink(lokiSettings, environment.EnvironmentName);
        }
    }
}