using Reveries.Application.Books.Commands.SetBookSeries;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.BookSeries.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Console.Services;
using Spectre.Console;

namespace Reveries.Console.Handlers;

public class DatabaseTableHandler : BaseHandler
{
    public override MenuChoice MenuChoice => MenuChoice.BooksInDatabase;

    private readonly IBookQueryRepository _bookQueries;
    private readonly CreateSeriesService _createSeriesService;
    private readonly SetBookSeriesHandler _setBookSeriesCommandHandler;
    private readonly BookDisplayService _bookDisplayService;

    public DatabaseTableHandler(
        IBookQueryRepository bookQueries,
        CreateSeriesService createSeriesService,
        SetBookSeriesHandler setBookSeriesCommandHandler,
        BookDisplayService bookDisplayService)
    {
        _bookQueries = bookQueries;
        _createSeriesService = createSeriesService;
        _setBookSeriesCommandHandler = setBookSeriesCommandHandler;
        _bookDisplayService = bookDisplayService;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var (booksInDb, elapsedSearchMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(() => _bookQueries.GetAllBooksAsync(ct));
        var sortedBooks = booksInDb.Arrange();

        AnsiConsole.MarkupLine($"\nElapsed search time: {elapsedSearchMs} ms".Italic().AsInfo());

        _bookDisplayService.DisplayBooksTable(sortedBooks);

        var menu = new Dictionary<string, Func<CancellationToken, Task>>
        {
            ["Add books to existing series"] = token => UpdateSelectedBooksWithSeriesAsync(sortedBooks, token),
            ["Go back"] = _ => Task.CompletedTask
        };

        var selection = ConsolePromptUtility.ShowSelectionPrompt("What do you want to do?", menu.Keys.ToArray());
        await menu[selection](ct);
    }

    private async Task UpdateSelectedBooksWithSeriesAsync(List<BookDetails> books, CancellationToken ct)
    {
        var seriesInDb = await _createSeriesService.GetSeriesAsync(ct);
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
            var numberInSeries = ConsolePromptUtility.GetUserInput(
                $"What number is {book.Title.AsSecondary()} in the series?");

            int? number = int.TryParse(numberInSeries, out var parsed) ? parsed : null;

            var bookSeriesCommand = new SetBookSeriesCommand(book.Isbn13!, series.Name, number);
            await _setBookSeriesCommandHandler.Handle(bookSeriesCommand, ct);
        }

        AnsiConsole.MarkupLine("\nThe following books have been updated:".AsSuccess());

        _bookDisplayService.DisplayBooksTable(selectedBooks);
    }
}