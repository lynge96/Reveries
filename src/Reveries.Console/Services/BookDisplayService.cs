using Reveries.Console.Common.Extensions;
using Reveries.Console.Interfaces;
using Reveries.Core.Enums;
using Reveries.Core.Helpers;
using Reveries.Core.Models;
using Spectre.Console;

namespace Reveries.Console.Services;

public class BookDisplayService : IBookDisplayService
{
    public void DisplayBooksTree(List<Book> books)
    {
        var root = new Tree($"Success! Found {books.Count.Bold().AsWarning()} book{(books.Count != 1 ? "s" : "")}:".AsSuccess().Underline());
        
        if (books.Count == 0)
        {
            root.AddNode("No books found".AsWarning());
            return;
        }
        
        foreach (var book in books)
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
        
        AnsiConsole.Write(root);
    }
    
    public void DisplayBooksTable(List<Book> books)
    {
        if (books.Count == 0)
        {
            AnsiConsole.MarkupLine("No books found.".AsWarning());
            return;
        }
        
        var table = new Table()
            .Border(TableBorder.SimpleHeavy)
            .BorderColor(Color.Yellow);

        var columnNames = new[]
        {
            "#", "ISBN", "Read", "Title", "Author", "Pages", "Published", 
            "Publisher", "#", "Series", "Binding", "Data source"
        };
        table.AddColumns(columnNames.Select(c => c.Bold().AsPrimary()).ToArray());

        for (var i = 0; i < books.Count; i++)
        {
            var book = books[i];
            table.AddRow(
                (i + 1).ToString().AsInfo(),
                book.Isbn13 ?? book.Isbn10 ?? "",
                book.IsRead ? "✅" : "❌",
                Markup.Escape(book.Title).Bold().AsSecondary(),
                Markup.Escape(book.GetAuthorNames()),
                book.Pages?.ToString() ?? "",
                book.PublishDateFormatted,
                Markup.Escape(book.Publisher?.Name ?? ""),
                book.SeriesNumber?.ToString() ?? "",
                book.Series != null
                    ? $"{Markup.Escape(book.Series.Name)} {Markup.Escape(book.Series.Id.ToString()).AsInfo()}"
                    : "",
                Markup.Escape(book.Binding ?? ""),
                book.DataSource.ToString().AsInfo()
            );
        }
        
        var totalPages = books.Sum(b => b.Pages ?? 0);
        var avgPages = books.Count > 0 ? totalPages / books.Count : 0;
        
        table.Columns[5].Footer($"Pages: {totalPages}".Bold().AsSecondary());
        table.Columns[6].Footer($"Avg. pages: {avgPages:N0}".Bold().AsSecondary());

        AnsiConsole.Write(table);
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
