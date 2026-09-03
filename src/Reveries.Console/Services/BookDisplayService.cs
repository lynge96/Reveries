using Reveries.Application.Books.Models;
using Reveries.Console.Common.Extensions;
using Spectre.Console;

namespace Reveries.Console.Services;

public class BookDisplayService
{
    public void DisplayBooksTree(IReadOnlyList<IBookRow> books)
    {
        var root = new Tree($"Success! Found {books.Count.Bold().AsWarning()} book{(books.Count != 1 ? "s" : "")}:".AsSuccess().Underline());

        if (books.Count == 0)
        {
            root.AddNode("No books found".AsWarning());
            return;
        }

        foreach (var book in books)
        {
            var bookNode = root.AddNode("📖 " + Markup.Escape(book.Title).Bold().AsPrimary());
            AddBookDetails(bookNode, book);
        }

        AnsiConsole.Write(root);
    }

    public void DisplayBooksTable(IReadOnlyList<IBookRow> books)
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
            "#", "ISBN", "Title", "Author", "Pages", "Published",
            "Publisher", "#", "Series", "Format"
        };
        table.AddColumns(columnNames.Select(c => c.Bold().AsPrimary()).ToArray());

        for (var i = 0; i < books.Count; i++)
        {
            var book = books[i];

            table.AddRow(
                (i + 1).ToString().AsInfo(),
                book.Isbn13 ?? "",
                Markup.Escape(book.Title).Bold().AsSecondary(),
                Markup.Escape(GetAuthorNames(book)),
                book.Pages?.ToString() ?? "",
                book.PublicationDate ?? "Unknown date",
                Markup.Escape(book.PublisherName ?? ""),
                book.SeriesNumber?.ToString() ?? "",
                Markup.Escape(book.SeriesName ?? ""),
                Markup.Escape(book.FormatLabel)
            );
        }

        var totalPages = books.Sum(b => b.Pages ?? 0);
        var avgPages = books.Count > 0 ? totalPages / books.Count : 0;

        table.Columns[4].Footer($"Pages: {totalPages}".Bold().AsSecondary());
        table.Columns[5].Footer($"Avg. pages: {avgPages:N0}".Bold().AsSecondary());

        AnsiConsole.Write(table);
    }

    private static string GetAuthorNames(IBookRow book)
    {
        return string.Join(", ", book.AuthorNames);
    }

    private static void AddBookDetails(TreeNode bookNode, IBookRow book)
    {
        var details = new Dictionary<string, string>
        {
            { "Author", string.Join(", ", book.AuthorNames) },
            { "Pages", book.Pages?.ToString() ?? "Unknown" },
            { "ISBN-10", book.Isbn10 ?? "N/A" },
            { "ISBN-13", book.Isbn13 ?? "N/A" },
            { "Publisher", book.PublisherName ?? "Unknown" },
            { "Language", book.LanguageLabel ?? "Unknown language" },
            { "Published", book.PublicationDate ?? "Unknown date" },
            { "Format", book.FormatLabel }
        };

        foreach (var (property, value) in details)
        {
            bookNode.AddNode($"{property}: {Markup.Escape(value).AsSecondary()}");
        }
    }
}
