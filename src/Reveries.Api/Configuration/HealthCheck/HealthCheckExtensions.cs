using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Reveries.Infrastructure.Postgresql.Configuration;
using Reveries.Infrastructure.Redis.Configuration;

namespace Reveries.Api.Configuration.HealthCheck;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddApplicationHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddPostgresqlHealthCheck()
            .AddRedisHealthCheck();
        
        return services;
    }
    
    public static WebApplication MapStandardHealthChecks(
        this WebApplication app, 
        string endpoint)
    {
        app.MapHealthChecks(endpoint, new HealthCheckOptions
        {
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
            },
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(report.Status.ToString());
            },
            AllowCachingResponses = false
        });

        return app;
    }
}