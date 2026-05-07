using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Reveries.Infrastructure.Redis.Configuration;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _redis;

    public RedisHealthCheck(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_redis.IsConnected)
            {
                return HealthCheckResult.Degraded("Redis is not connected");
            }

            var database = _redis.GetDatabase();
            await  database.PingAsync();
            
            return HealthCheckResult.Healthy("Redis connection is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Degraded(
                "Redis connection failed",
                exception: ex);
        }
    }
}