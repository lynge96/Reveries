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
    
    public async Task<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        var value = await _database.StringGetAsync(key);
        
        if (value.IsNullOrEmpty) return default;
        
        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
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