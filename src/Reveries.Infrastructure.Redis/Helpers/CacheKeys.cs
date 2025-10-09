using Reveries.Core.Helpers;

namespace Reveries.Infrastructure.Redis.Helpers;

public static class CacheKeys
{
    public static string BookByIsbn(string isbn) => $"book:by_isbn:{isbn}";
    public static string BookIsbnsByTitle(string title) => $"book:isbn_by_title:{title.ToTitleCase()}";
}
