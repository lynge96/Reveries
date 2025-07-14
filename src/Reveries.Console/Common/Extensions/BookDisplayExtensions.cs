using Reveries.Core.Models;
using Spectre.Console;

namespace Reveries.Console.Common.Extensions;

public static class BookDisplayExtensions
{
    public static Tree DisplayBooks(this BooksListResponse bookCollection)
    {
        var root = new Tree($"Success! Found {bookCollection.Total.Bold().AsWarning()} of {bookCollection.Requested.Bold().AsWarning()} requested book{(bookCollection.Total != 1 ? "s" : "")} in the database:".AsSuccess());
        
        if (bookCollection.Books.Any() != true)
        {
            root.AddNode("No books found".AsWarning());
            return root;
        }

        foreach (var book in bookCollection.Books)
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

}