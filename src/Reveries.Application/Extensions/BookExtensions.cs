using Reveries.Core.Enums;
using Reveries.Core.Models;

namespace Reveries.Application.Extensions;


public static class BookExtensions
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
    
    public static string? GetIsbnKey(Book book)
    {
        if (!string.IsNullOrWhiteSpace(book.Isbn13))
            return book.Isbn13;
        if (!string.IsNullOrWhiteSpace(book.Isbn10))
            return book.Isbn10;
        return null;
    }
}