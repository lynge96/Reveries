using Reveries.Core.Models;
using Spectre.Console;

namespace Reveries.Console.Extensions;

public static class BookDisplayExtensions
{
    public static Tree DisplayBooks(this IEnumerable<Book> books)
    {
        var bookList = books.ToList();
        var bookCount = bookList.Count;
        var root = new Tree($"Success! Found {bookCount} book{(bookCount != 1 ? "s" : "")} in the database:".AsSuccess());

        foreach (var book in bookList)
        {
            var bookNode = root.AddNode("📖 " + book.Title.Bold().AsPrimary());
            var details = new Dictionary<string, string>
            {
                { "Author", string.Join(", ", book.Authors) },
                { "Pages", book.Pages?.ToString() ?? "Unknown" },
                { "ISBN-10", book.Isbn10 },
                { "ISBN-13", book.Isbn13 ?? "N/A" },
                { "Publisher", book.Publisher ?? "Unknown" },
                { "Language", book.Language ?? "Unknown language" },
                { "Published", book.PublishDate?.ToString("dd MMM yyyy") ?? "Unknown" },
                { "MSRP", book.Msrp?.ToString() ?? "Unknown" },
                { "Binding", book.Binding ?? "Unknown" }
            };

            foreach (var (property, value) in details)
            {
                bookNode.AddNode($"{property}: {value.AsSecondary()}");
            }

        }

        return root;
    }


    
    public static Tree DisplayBook(this Book book)
        => new[] { book }.DisplayBooks();
}