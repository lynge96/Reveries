using System.Text.Json;
using Reveries.Application.Interfaces.Cache;
using StackExchange.Redis;

namespace Reveries.Infrastructure.Redis.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }
    
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _database.StringGetAsync(key);
        
        if (value.IsNullOrEmpty) return default;
        
        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task<IReadOnlyList<T>> GetAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
        var values = await _database.StringGetAsync(redisKeys);

        var list = new List<T>();
        
        foreach (var value in values)
        {
            if (value.HasValue)
            {
                var item = JsonSerializer.Deserialize<T>(value!);
                if (item != null) list.Add(item);
            }
        }

        return list;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value);
        
        await _database.StringSetAsync(key, json, expiry);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _database.KeyDeleteAsync(key);
    }
}