using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Books.Caching;
using Reveries.Application.Books.Interfaces;

namespace Reveries.Application.Common.Caching;

public static class CachingServiceCollectionExtensions
{
    public static IServiceCollection AddBookSearchCaching(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<CacheSettings>()
            .Bind(config.GetSection(CacheSettings.SectionName))
            .Validate(s => s.IsbnLookupTtlHours > 0, "Cache: IsbnLookupTtlHours must be positive")
            .ValidateOnStart();

        services.AddHybridCache();

        services.Decorate<IBookSearch, CachingBookSearch>();

        return services;
    }
}
