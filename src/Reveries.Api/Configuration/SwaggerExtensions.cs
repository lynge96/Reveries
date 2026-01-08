using Microsoft.OpenApi;

namespace Reveries.Api.Configuration;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Reveries API",
                Version = "v1",
                Description = "REST API for the Reveries application"
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.DocumentTitle = "Reveries API";
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Reveries API v1");
            options.RoutePrefix = string.Empty;
        });

        return app;
    }
}