using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reveries.Application.Interfaces.Cache;
using Reveries.Core.Configuration;
using Reveries.Infrastructure.Redis.Services;
using StackExchange.Redis;

namespace Reveries.Infrastructure.Redis.Configuration;

public static class RedisServiceCollectionExtensions
{
    public static IServiceCollection AddRedisCacheServices(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<RedisSettings>>().Value;
            var connectionString = settings.GetConnectionString();
            
            return ConnectionMultiplexer.Connect(connectionString);
        });
        
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IBookCacheService, BookCacheService>();
        
        return services;
    }
}