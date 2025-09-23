using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reveries.Application.Interfaces.Cache;
using Reveries.Infrastructure.Redis.Services;
using StackExchange.Redis;

namespace Reveries.Infrastructure.Redis.Configuration;

public static class RedisServiceCollectionExtensions
{
    public static IServiceCollection AddRedisCacheServices(this IServiceCollection services)
    {
        services.Configure<RedisSettings>(options =>
        {
            options.ConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") 
                             ?? throw new InvalidOperationException("REDIS_CONNECTION_STRING missing");
        });

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
            return ConnectionMultiplexer.Connect(settings.ConnectionString);
        });
        
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IBookCacheService, BookCacheService>();
        
        return services;
    }
}