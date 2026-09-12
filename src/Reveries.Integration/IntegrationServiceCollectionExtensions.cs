using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reveries.Integration.GoogleBooks.Configuration;
using Reveries.Integration.Isbndb.Configuration;

namespace Reveries.Integration;

public static class IntegrationServiceCollectionExtensions
{
    public static IServiceCollection AddIntegration(this IServiceCollection services, IConfiguration config)
    {
        services.AddIsbndb(config);
        services.AddGoogleBooks(config);

        return services;
    }
}