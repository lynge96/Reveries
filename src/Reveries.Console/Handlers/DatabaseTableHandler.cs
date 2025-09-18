using Reveries.Application.Extensions;
using Reveries.Application.Interfaces.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Console.Services.Interfaces;
using Reveries.Core.Entities;
using Spectre.Console;

namespace Reveries.Console.Handlers;

public class DatabaseTableHandler : BaseHandler
{
    public override MenuChoice MenuChoice => MenuChoice.BooksInDatabase;
    private readonly IBookLookupService _bookLookupService;
    private readonly IBookSeriesService _bookSeriesService;
    private readonly IBookManagementService _bookManagementService;
    private readonly IBookDisplayService _bookDisplayService;

    public DatabaseTableHandler(IBookLookupService bookLookupService, IBookSeriesService bookSeriesService, IBookManagementService bookManagementService, IBookDisplayService bookDisplayService)
    {
        _bookLookupService = bookLookupService;
        _bookSeriesService = bookSeriesService;
        _bookManagementService = bookManagementService;
        _bookDisplayService = bookDisplayService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var booksInDb = await _bookLookupService.GetAllBooksAsync(cancellationToken);
        var sortedBooks = booksInDb.ArrangeBooks();
        
        _bookDisplayService.DisplayBooksTable(sortedBooks);
        
        var menu = new Dictionary<string, Func<CancellationToken, Task>>
        {
            ["Add books to existing series"] = ct => UpdateSelectedBooksWithSeriesAsync(sortedBooks, ct),
            ["Update read status"] = ct => UpdateSelectedBooksReadStatusAsync(sortedBooks, ct),
            ["Go back"] = _ => Task.CompletedTask
        };

        var selection = ConsolePromptUtility.ShowSelectionPrompt("What do you want to do?", menu.Keys.ToArray());
        await menu[selection](cancellationToken);
    }

    private async Task UpdateSelectedBooksWithSeriesAsync(List<Book> books, CancellationToken cancellationToken)
    {
        var seriesInDb = await _bookSeriesService.GetSeriesAsync();
        if (seriesInDb.Count == 0)
        {
            AnsiConsole.MarkupLine("No series found in database.".AsWarning());
            return;
        }
        
        var series = ConsolePromptUtility.ShowSelectionPrompt(
            "Select the series you want to add books to:", seriesInDb);

        var selectedBooks = ConsolePromptUtility.ShowMultiSelectionPrompt("Select the books you want to add:", books);
        if (selectedBooks.Count == 0)
        {
            AnsiConsole.MarkupLine("No books selected.".AsWarning());
            return;
        }
        
        foreach (var book in selectedBooks)
        {
            book.Series = series;

            var numberInSeries = ConsolePromptUtility.GetUserInput(
                $"What number is {book.Title.AsSecondary()} in the series?");
            if (int.TryParse(numberInSeries, out var num))
                book.SeriesNumber = num;
        }
            
        await _bookManagementService.UpdateBooksAsync(selectedBooks, cancellationToken);

        AnsiConsole.MarkupLine("\nThe following books have been updated:".AsSuccess());
        
        _bookDisplayService.DisplayBooksTable(selectedBooks);
    }

    private async Task UpdateSelectedBooksReadStatusAsync(List<Book> books, CancellationToken cancellationToken)
    {
        var selectedBooks = ConsolePromptUtility.ShowMultiSelectionPrompt("Select the books you want update:", books);
        if (selectedBooks.Count == 0)
        {
            AnsiConsole.MarkupLine("No books selected.".AsWarning());
            return;
        }
        
        foreach (var book in selectedBooks)
        {
            book.IsRead = !book.IsRead;
        }
        
        await _bookManagementService.UpdateBooksAsync(selectedBooks, cancellationToken);
        
        AnsiConsole.MarkupLine("\nThe following books have been updated:".AsSuccess());
        
        _bookDisplayService.DisplayBooksTable(selectedBooks);
    }
}