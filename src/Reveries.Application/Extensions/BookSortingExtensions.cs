using Reveries.Core.Entities;
using Reveries.Core.Enums;

namespace Reveries.Application.Extensions;


public static class BookSortingExtensions
{
    public static List<Book> ArrangeBooks(this IEnumerable<Book> books)
    {
        return books
            .OrderBy(b => b.DataSource == DataSource.Cache)
            .ThenByDescending(b => b.DataSource == DataSource.Database)
            .ThenBy(b => b.DataSource == DataSource.CombinedBookApi)
            .ThenBy(b => b.Authors.FirstOrDefault()?.FirstName)
            .ThenBy(b => b.SeriesNumber)
            .ThenBy(b => b.Title)
            .ToList();
    }
}