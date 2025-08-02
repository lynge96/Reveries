using Reveries.Core.Entities;
using Spectre.Console;

namespace Reveries.Console.Common.Extensions;

public static class BookDisplayExtensions
{
    public static Tree DisplayBooks(this List<Book> books)
    {
        var root = new Tree($"Success! Found {books.Count.Bold().AsWarning()} book{(books.Count != 1 ? "s" : "")} in the database:".AsSuccess().Underline());
        
        if (books.Any() == false)
        {
            root.AddNode("No books found".AsWarning());
            return root;
        }

        foreach (var book in books)
        {
            var bookNode = root.AddNode("📖 " + Markup.Escape(book.Title).Bold().AsPrimary());
            var details = new Dictionary<string, string>
            {
                { "Author", string.Join(", ", book.Authors.Select(author => author.NormalizedName)) },
                { "Pages", book.Pages?.ToString() ?? "Unknown" },
                { "ISBN-10", book.Isbn10 ?? "N/A"},
                { "ISBN-13", book.Isbn13 ?? "N/A" },
                { "Publisher", book.Publisher?.Name ?? "Unknown" },
                { "Language", book.Language ?? "Unknown language" },
                { "Published", book.PublishDate?.ToString("dd MMM yyyy") ?? "Unknown" },
                { "MSRP", book.Msrp?.ToString() ?? "Unknown" },
                { "Binding", book.Binding ?? "Unknown" }
            };

            foreach (var (property, value) in details)
            {
                bookNode.AddNode($"{property}: {Markup.Escape(value).AsSecondary()}");
            }
        }
        return root;
    }

}