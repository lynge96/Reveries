namespace Reveries.Infrastructure.Redis.Configuration;

public class RedisSettings
{
    public static TimeSpan? Expiration { get; } = TimeSpan.FromMinutes(10);
    public string ConnectionString { get; set; } = string.Empty;
}