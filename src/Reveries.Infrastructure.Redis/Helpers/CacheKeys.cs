namespace Reveries.Infrastructure.Redis.Helpers;

public static class CacheKeys
{
    public static string BookByIsbn(string isbn) => $"books:{isbn}";
}
