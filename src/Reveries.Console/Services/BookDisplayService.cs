using Reveries.Application.Books.Models;
using Reveries.Console.Common.Extensions;
using Reveries.Domain.Enums;
using Reveries.Domain.Helpers;
using Spectre.Console;

namespace Reveries.Console.Services;

public class BookDisplayService
{
    public void DisplayBooksTree(List<EditionWithWork> books)
    {
        var root = new Tree($"Success! Found {books.Count.Bold().AsWarning()} book{(books.Count != 1 ? "s" : "")}:".AsSuccess().Underline());

        if (books.Count == 0)
        {
            root.AddNode("No books found".AsWarning());
            return;
        }

        foreach (var book in books)
        {
            var sourceLabel = book.Edition.DataSource switch
            {
                DataSource.Database => " (Database)",
                DataSource.GoogleBooksApi => " (GoogleBooks API)",
                DataSource.IsbndbApi => " (ISBNDB API)",
                DataSource.CombinedBookApi => " (Combined API)",
                DataSource.Cache => " (Cache)",
                _ => ""
            };
            var bookNode = root.AddNode("📖 " + Markup.Escape(book.Work.Title.Text).Bold().AsPrimary() + sourceLabel.AsInfo());
            AddBookDetails(bookNode, book);
        }

        AnsiConsole.Write(root);
    }

    public void DisplayBooksTable(List<EditionWithWork> books)
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
            "Publisher", "#", "Series", "Format", "Data source"
        };
        table.AddColumns(columnNames.Select(c => c.Bold().AsPrimary()).ToArray());

        for (var i = 0; i < books.Count; i++)
        {
            var book = books[i];
            var edition = book.Edition;
            var work = book.Work;

            table.AddRow(
                (i + 1).ToString().AsInfo(),
                edition.Isbn?.Value13 ?? "",
                Markup.Escape(work.Title.Text).Bold().AsSecondary(),
                Markup.Escape(GetAuthorNames(book)),
                edition.Pages?.ToString() ?? "",
                edition.PublicationDate?.Value ?? "Unknown date",
                Markup.Escape(edition.Publisher?.Name ?? ""),
                work.SeriesPlacement?.Number?.ToString() ?? "",
                work.SeriesPlacement != null
                    ? $"{Markup.Escape(work.SeriesPlacement.Series.Name)} {Markup.Escape(work.SeriesPlacement.Series.Id.ToString()).AsInfo()}"
                    : "",
                Markup.Escape(edition.Format.ToString()),
                edition.DataSource.ToString().AsInfo()
            );
        }

        var totalPages = books.Sum(b => b.Edition.Pages ?? 0);
        var avgPages = books.Count > 0 ? totalPages / books.Count : 0;

        table.Columns[4].Footer($"Pages: {totalPages}".Bold().AsSecondary());
        table.Columns[5].Footer($"Avg. pages: {avgPages:N0}".Bold().AsSecondary());

        AnsiConsole.Write(table);
    }

    private static string GetAuthorNames(EditionWithWork book)
        => string.Join(", ", book.Work.Authors.Select(a => a.ToString()));

    private static void AddBookDetails(TreeNode bookNode, EditionWithWork book)
    {
        var edition = book.Edition;
        var work = book.Work;

        var details = new Dictionary<string, string>
        {
            { "Author", string.Join(", ", work.Authors.Select(author => author.Name)) },
            { "Pages", edition.Pages?.ToString() ?? "Unknown" },
            { "ISBN-10", edition.Isbn?.Value10 ?? "N/A"},
            { "ISBN-13", edition.Isbn?.Value13 ?? "N/A" },
            { "Publisher", edition.Publisher?.Name ?? "Unknown" },
            { "Language", edition.Language?.DisplayName ?? "Unknown language" },
            { "Published", edition.PublicationDate?.Value ?? "Unknown date" },
            { "MSRP", edition.Msrp?.ToString() ?? "Unknown" },
            { "Format", edition.Format.ToString() }
        };

        foreach (var (property, value) in details)
        {
            bookNode.AddNode($"{property}: {Markup.Escape(value).AsSecondary()}");
        }
    }
}
