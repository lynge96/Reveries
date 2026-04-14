using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reveries.Integration.GoogleBooks.Configuration;
using Reveries.Integration.Isbndb.Configuration;

namespace Reveries.Integration;

public static class DependencyInjection
{
    public static IServiceCollection AddIntegrations(this IServiceCollection services, IConfiguration config)
    {
        services.AddGoogleBooks(config);
        services.AddIsbndb(config);
        
        return services;
    }
}