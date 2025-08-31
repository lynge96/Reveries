using Reveries.Application.Interfaces.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Console.Services.Interfaces;
using Spectre.Console;

namespace Reveries.Console.Handlers;

public class SearchPublisherHandler : BaseHandler
{
    public override MenuChoice MenuChoice => MenuChoice.SearchPublisher;
    private readonly IPublisherService _publisherService;
    private readonly IBookSelectionService _bookSelectionService;
    private readonly IBookDisplayService _bookDisplayService;

    public SearchPublisherHandler(IPublisherService publisherService, IBookSelectionService bookSelectionService, IBookDisplayService bookDisplayService)
    {
        _publisherService = publisherService;
        _bookSelectionService = bookSelectionService;
        _bookDisplayService = bookDisplayService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var publisherInput = ConsolePromptUtility.GetUserInput("Enter publisher name:");
        
        var (publishers, elapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(() => _publisherService.GetPublishersByNameAsync(publisherInput, cancellationToken));

        AnsiConsole.MarkupLine($"\nElapsed search time: {elapsedMs} ms".Italic().AsInfo());
        
        var selectedPublisher = ConsolePromptUtility.ShowSelectionPrompt("Select a publisher to see their books:", publishers);
        
        var (bookResults, bookSearchElapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(async () => await _publisherService.GetBooksByPublisherAsync(selectedPublisher, cancellationToken));
        
        if (bookResults.Count == 0)
        {
            AnsiConsole.MarkupLine($"No books found for publisher: {selectedPublisher.AsSecondary()}".AsWarning());
            return;
        }

        var filteredBooks = _bookSelectionService.FilterBooksByLanguage(bookResults);
        
        AnsiConsole.MarkupLine($"Elapsed book search time: {bookSearchElapsedMs} ms".Italic().AsInfo());
        AnsiConsole.Write(_bookDisplayService.DisplayBooks(filteredBooks));
    }
}