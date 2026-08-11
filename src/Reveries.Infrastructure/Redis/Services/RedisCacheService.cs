using System.Text.Json;
using Reveries.Infrastructure.Redis.Interfaces;
using StackExchange.Redis;

namespace Reveries.Infrastructure.Redis.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _database;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }
    
    public async Task<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        var value = await _database.StringGetAsync(key);

        if (value.IsNullOrEmpty)
            return default;

        var json = (string)value!;
        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiry, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(value);
        
        await _database.StringSetAsync(key, json, expiry);
    }

    public async Task RemoveAsync(string key, CancellationToken ct)
    {
        await _database.KeyDeleteAsync(key);
    }

    public IBatch CreateBatch() => _database.CreateBatch();
}