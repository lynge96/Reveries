using Reveries.Application.Configuration;
using Reveries.Infrastructure.Postgresql.Configuration;
using Reveries.Infrastructure.Redis.Configuration;
using Reveries.Integration.GoogleBooks.Configuration;
using Reveries.Integration.Isbndb.Configuration;

namespace Reveries.Api.Configuration;

public static class ReveriesServicesExtensions
{
    public static IServiceCollection AddReveriesServices(this IServiceCollection services)
    {
        services
            .AddApplicationServices()
            .AddInfrastructureServices()
            .AddIsbndbServices()
            .AddGoogleBooksServices()
            .AddRedisCacheServices();
        
        return services;
    }
}