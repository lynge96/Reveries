using Microsoft.Extensions.DependencyInjection;
using Reveries.Infrastructure.Postgresql.Configuration;
using Reveries.Infrastructure.Redis.Configuration;

namespace Reveries.Infrastructure.Configuration;

public static class ServiceCollectionExtensions
{
    
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddPostgresql();
        services.AddRedisCache();

        return services;
    }
}