using Reveries.Core.Entities;
using Reveries.Core.Enums;

namespace Reveries.Application.Extensions;

public static class BookFormatExtensions
{
    private static readonly HashSet<string> PhysicalFormats = new(StringComparer.OrdinalIgnoreCase)
    {
        "Paperback",
        "Hardback",
        "Hardcover",
        "Hard Cover",
        "Trade Paperback",
        "Mass Market Paperback",
        "Library Binding",
        "Board Book",
        "Leather Bound"
    };
    
    public static IEnumerable<Book> FilterByFormat(this IEnumerable<Book> books, BookFormat format)
    {
        return format switch
        {
            BookFormat.PhysicalOnly => books.Where(book => IsPhysicalBook(book.Binding)),
            BookFormat.DigitalOnly => books.Where(book => !IsPhysicalBook(book.Binding)),
            _ => books
        };
    }

    private static bool IsPhysicalBook(this string? binding) => 
        binding != null && PhysicalFormats.Contains(binding);

}