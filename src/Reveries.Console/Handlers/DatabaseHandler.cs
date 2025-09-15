using Reveries.Application.Interfaces.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Spectre.Console;

namespace Reveries.Console.Handlers;

public class DatabaseHandler : BaseHandler
{
    public override MenuChoice MenuChoice => MenuChoice.BooksInDatabase;
    private readonly IBookLookupService _bookLookupService;

    public DatabaseHandler(IBookLookupService bookLookupService)
    {
        _bookLookupService = bookLookupService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var booksInDb = await _bookLookupService.GetAllBooksAsync(cancellationToken);
        
        var table = new Table()
            .Border(TableBorder.Horizontal)
            .BorderColor(Color.Yellow)
            .Title("Reveries Database".AsHeader().Bold());

        var columnNames = new[]
        {
            "ISBN", "Read", "Title", "Author", "Published", 
            "Publisher", "#", "Series", "Binding", "Pages"
        };
        table.AddColumns(columnNames.Select(c => c.Bold().AsPrimary()).ToArray());
        
        foreach (var book in booksInDb)
        {
            table.AddRow(
                book.Isbn13 ?? book.Isbn10 ?? "",
                book.IsRead ? "✅" : "❌",
                book.Title.Bold(), 
                book.GetAuthorNames(), 
                book?.PublishDateFormatted ?? "", 
                book?.Publisher?.Name ?? "",
                book?.SeriesNumber.ToString() ?? "",
                book?.Series?.Name ?? "",
                book?.Binding ?? "",
                book?.Pages.ToString() ?? "").Collapse();
        }
        
        var bookCount = booksInDb.Count;
        table.Columns[0].Footer($"Books in total: {bookCount}".Bold().AsSecondary());
        var totalPages = booksInDb.Sum(b => b.Pages ?? 0);
        table.Columns[columnNames.Length-3].Footer($"Total pages: {totalPages}".Bold().AsSecondary());
        var avgPages = totalPages / bookCount;
        table.Columns[columnNames.Length-2].Footer($"Average pages: {avgPages:N0}".Bold().AsSecondary());
        
        AnsiConsole.Write(table);
    }
}