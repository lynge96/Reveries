using Reveries.Application.Books.Models;

namespace Reveries.Console.Common.Extensions;

public static class BookRowExtensions
{
    public static List<T> Arrange<T>(this IEnumerable<T> items) where T : IBookRow
    {
        return items
            .OrderBy(x => x.AuthorNames.FirstOrDefault())
            .ThenBy(x => x.SeriesNumber)
            .ThenBy(x => x.Title)
            .ToList();
    }
}