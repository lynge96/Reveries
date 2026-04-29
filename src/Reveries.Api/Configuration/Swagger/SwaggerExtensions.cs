using Microsoft.OpenApi;

namespace Reveries.Api.Configuration.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        var swaggerConfig = configuration.GetSection("Swagger").Get<SwaggerConfiguration>() 
                            ?? new SwaggerConfiguration();
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(swaggerConfig.Version, new OpenApiInfo
            {
                Title = swaggerConfig.Title,
                Version = swaggerConfig.Version,
                Description = swaggerConfig.Description
            });
            
            options.EnableAnnotations();
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IConfiguration configuration)
    {
        var swaggerConfig = configuration.GetSection("Swagger").Get<SwaggerConfiguration>() 
                            ?? new SwaggerConfiguration();
        
        app.UseSwagger(c =>
        {
            c.PreSerializeFilters.Add((swagger, httpReq) =>
            {
                if (swaggerConfig.Servers?.Count > 0)
                {
                    swagger.Servers = swaggerConfig.Servers.Select(s => new OpenApiServer
                    {
                        Url = s.Url,
                        Description = s.Description
                    }).ToList();
                }
            });
        });
        app.UseSwaggerUI(options =>
        {
            options.DocumentTitle = swaggerConfig.Title;
            options.SwaggerEndpoint($"/swagger/{swaggerConfig.Version}/swagger.json", 
                $"{swaggerConfig.Title} {swaggerConfig.Version}");
            options.RoutePrefix = string.Empty;
            
            options.DisplayRequestDuration();
            options.EnableDeepLinking();
        });

        return app;
    }
}