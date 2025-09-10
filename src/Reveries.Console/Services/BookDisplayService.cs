using System.Globalization;
using Reveries.Application.Extensions;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Services.Interfaces;
using Reveries.Core.Entities;
using Reveries.Core.Enums;
using Spectre.Console;

namespace Reveries.Console.Services;

public class BookDisplayService : IBookDisplayService
{
    public Tree DisplayBooks(List<Book> books)
    {
        var root = new Tree($"Success! Found {books.Count.Bold().AsWarning()} book{(books.Count != 1 ? "s" : "")}:".AsSuccess().Underline());
        
        if (books.Count == 0)
        {
            root.AddNode("No books found".AsWarning());
            return root;
        }

        var sortedBooks = books
            .OrderByDescending(b => b.DataSource.HasFlag(DataSource.Database))
            .ThenBy(b => b.DataSource.HasFlag(DataSource.CombinedBookApi))
            .ThenBy(b => b.Title)
            .ToList();
        
        foreach (var book in sortedBooks)
        {
            var sourceLabel = book.DataSource switch
            {
                DataSource.Database => " (Database)",
                DataSource.GoogleBooksApi => " (GoogleBooks API)",
                DataSource.IsbndbApi => " (ISBNDB API)",
                DataSource.CombinedBookApi => " (Combined API)",
                DataSource.Cache => " (Cache)",
                _ => ""
            };
            
            var bookNode = root.AddNode("📖 " + Markup.Escape(book.Title).Bold().AsPrimary() + sourceLabel.AsInfo());
            AddBookDetails(bookNode, book);
        }
        
        return root;
    }

    private static void AddBookDetails(TreeNode bookNode, Book book)
    {
        var details = new Dictionary<string, string>
        {
            { "Author", string.Join(", ", book.Authors.Select(author => author.NormalizedName.ToTitleCase())) },
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
}
