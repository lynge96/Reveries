using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reveries.Application.Interfaces.Cache;
using Reveries.Infrastructure.Redis.Interfaces;
using Reveries.Infrastructure.Redis.Services;
using StackExchange.Redis;

namespace Reveries.Infrastructure.Redis.Configuration;

public static class RedisServiceCollectionExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<RedisSettings>()
            .Bind(config.GetSection("Redis"))
            .ValidateOnStart();
        
        services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<RedisSettings>>().Value;
            var connectionString = settings.GetConnectionString();
            
            return ConnectionMultiplexer.Connect(connectionString);
        });
        
        services.AddScoped<IRedisCacheService, RedisCacheService>();
        services.AddScoped<IBookCacheService, BookCacheService>();
        
        return services;
    }
}