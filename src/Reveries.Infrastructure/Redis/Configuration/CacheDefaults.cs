namespace Reveries.Infrastructure.Redis.Configuration;

public static class CacheDefaults
{
    public static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(10);
}