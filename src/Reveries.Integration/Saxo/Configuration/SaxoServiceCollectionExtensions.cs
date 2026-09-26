using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Books.Interfaces;
using Reveries.Integration.Saxo.Services;

namespace Reveries.Integration.Saxo.Configuration;

public static class SaxoServiceCollectionExtensions
{
    public static IServiceCollection AddSaxo(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<SaxoSettings>()
            .Bind(config.GetSection(SaxoSettings.SectionName))
            .Validate(s => !string.IsNullOrWhiteSpace(s.SearchUrlTemplate) && s.SearchUrlTemplate.Contains("{0}"),
                "Saxo: SearchUrlTemplate must be set and contain a '{0}' placeholder")
            .ValidateOnStart();

        services.AddScoped<ISaxoBookSearch, SaxoBookSearch>();

        return services;
    }
}