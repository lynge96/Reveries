using StackExchange.Redis;

namespace Reveries.Infrastructure.Redis.Interfaces;

public interface IRedisCacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan expiry, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
    IBatch CreateBatch();
}