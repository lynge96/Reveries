using Reveries.Core.Helpers;
using Reveries.Core.ValueObjects;

namespace Reveries.Infrastructure.Redis.Configuration;

public static class CacheKeys
{
    public static string BookByIsbn(Isbn isbn) => $"book:by_isbn:{isbn}";
    public static string BookIsbnsByTitle(string title) => $"book:isbn_by_title:{title.ToTitleCase()}";
}
