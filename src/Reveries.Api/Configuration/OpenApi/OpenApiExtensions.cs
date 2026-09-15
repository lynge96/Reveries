using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace Reveries.Api.Configuration.OpenApi;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiDocument(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection("OpenApi").Get<OpenApiConfiguration>()
                     ?? new OpenApiConfiguration();

        services.AddOpenApi(config.Version, options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = config.Title,
                    Version = config.Version,
                    Description = config.Description
                };

                if (config.Servers is { Count: > 0 })
                {
                    document.Servers = config.Servers
                        .Select(server => new OpenApiServer
                        {
                            Url = server.Url,
                            Description = server.Description
                        })
                        .ToList();
                }

                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static WebApplication MapOpenApiDocumentation(this WebApplication app, IConfiguration configuration)
    {
        var config = configuration.GetSection("OpenApi").Get<OpenApiConfiguration>()
                     ?? new OpenApiConfiguration();

        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = config.Title;
            options.OpenApiRoutePattern = $"/openapi/{config.Version}.json";
        });

        return app;
    }
}