using Reveries.Application.Interfaces.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Core.Entities;
using Spectre.Console;

namespace Reveries.Console.Handlers;

public class DatabaseTableHandler : BaseHandler
{
    public override MenuChoice MenuChoice => MenuChoice.BooksInDatabase;
    private readonly IBookLookupService _bookLookupService;
    private readonly IBookSeriesService _bookSeriesService;
    private readonly IBookManagementService _bookManagementService;

    public DatabaseTableHandler(IBookLookupService bookLookupService, IBookSeriesService bookSeriesService, IBookManagementService bookManagementService)
    {
        _bookLookupService = bookLookupService;
        _bookSeriesService = bookSeriesService;
        _bookManagementService = bookManagementService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var booksInDb = await _bookLookupService.GetAllBooksAsync(cancellationToken);
        
        PrintBookTable(booksInDb);

        string[] options = ["Add books to existing series", "Go back"];
        var selection = ConsolePromptUtility.ShowSelectionPrompt("What do you want to do?", options);
        switch (selection)
        {
            case "Go back":
                return;
            case "Add books to existing series":
                var seriesInDb = await _bookSeriesService.GetSeriesAsync();
                if (seriesInDb.Count == 0)
                    return;
                
                var series = ConsolePromptUtility.ShowSelectionPrompt(
                    "Select the series you want to add books to:", seriesInDb);

                var bookNumbersInput =
                    ConsolePromptUtility.GetUserInput(
                        "Enter the book numbers you want to add to the series (comma separated):");
                var bookNumbers = bookNumbersInput.Split(',').Select(int.Parse).ToList();
                
                var updatedBooks = new List<Book>();
                
                foreach (var bookNumber in bookNumbers)
                {
                    var book = booksInDb[bookNumber - 1];
                    book.Series = series;

                    var numberInSeries = ConsolePromptUtility.GetUserInput($"What number is {book.Title.AsSecondary()} in the series?");
                    book.SeriesNumber = int.Parse(numberInSeries);
                    
                    updatedBooks.Add(book);
                }

                if (updatedBooks.Count == 0)
                    return;
                
                await _bookManagementService.UpdateBooksWithSeriesAsync(updatedBooks, cancellationToken);
                AnsiConsole.MarkupLine("\nThe following books have been updated:".AsSuccess());

                PrintBookTable(updatedBooks);
                
                break;
        }

    }

    private void PrintBookTable(List<Book> books)
    {
        var table = new Table()
            .Border(TableBorder.Horizontal)
            .BorderColor(Color.Yellow);

        var columnNames = new[]
        {
            "", "ISBN", "Read", "Title", "Author", "Pages", "Published", 
            "Publisher", "#", "Series", "Binding"
        };
        table.AddColumns(columnNames.Select(c => c.Bold().AsPrimary()).ToArray());

        for (var i = 0; i < books.Count; i++)
        {
            var book = books[i];
            table.AddRow(
                    (i + 1).ToString().AsInfo(),
                    book.Isbn13 ?? book.Isbn10 ?? "",
                    book.IsRead ? "✅" : "❌",
                    book.Title.Bold().AsSecondary(),
                    book.GetAuthorNames(),
                    book?.Pages.ToString() ?? "",
                    book?.PublishDateFormatted ?? "",
                    book?.Publisher?.Name ?? "",
                    book?.SeriesNumber.ToString() ?? "",
                    book?.Series != null
                        ? $"{book.Series.Name} {book.Series.Id.ToString().AsInfo()}" 
                        : "",
                    book?.Binding ?? "")
                .Collapse();
        }
        var totalPages = books.Sum(b => b.Pages ?? 0);
        table.Columns[5].Footer($"Pages: {totalPages}".Bold().AsSecondary());
        var avgPages = totalPages / books.Count;
        table.Columns[6].Footer($"Avg. pages: {avgPages:N0}".Bold().AsSecondary());
        
        AnsiConsole.Write(table);
    }
}