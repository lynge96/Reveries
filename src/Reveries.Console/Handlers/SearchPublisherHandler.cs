using Reveries.Application.Extensions;
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
    private readonly IBookSelectionService _bookSelectionService;
    private readonly IBookDisplayService _bookDisplayService;
    private readonly IBookLookupService _bookLookupService;
    private readonly IPublisherLookupService _publisherLookupService;

    public SearchPublisherHandler(IBookSelectionService bookSelectionService, IBookDisplayService bookDisplayService, IBookLookupService bookLookupService, IPublisherLookupService publisherLookupService)
    {
        _bookSelectionService = bookSelectionService;
        _bookDisplayService = bookDisplayService;
        _bookLookupService = bookLookupService;
        _publisherLookupService = publisherLookupService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var publisherInput = ConsolePromptUtility.GetUserInput("Enter publisher name:");
        
        var (publishers, elapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(() => _publisherLookupService.FindPublishersByNameAsync(publisherInput, cancellationToken));

        AnsiConsole.MarkupLine($"\nElapsed search time: {elapsedMs} ms".Italic().AsInfo());
        
        var selectedPublisher = ConsolePromptUtility.ShowSelectionPrompt("Select a publisher to see their books:", publishers);
        
        var (bookResults, bookSearchElapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(async () => await _bookLookupService.FindBooksByPublisherAsync(selectedPublisher.Name, cancellationToken));
        
        if (bookResults.Count == 0)
        {
            if (selectedPublisher.Name != null)
                AnsiConsole.MarkupLine(
                    $"No books found for publisher: {selectedPublisher.Name.AsSecondary()}".AsWarning());
            return;
        }

        var filteredBooks = _bookSelectionService.FilterBooksByLanguage(bookResults);
        
        AnsiConsole.MarkupLine($"Elapsed book search time: {bookSearchElapsedMs} ms".Italic().AsInfo());
        
        _bookDisplayService.DisplayBooksTable(filteredBooks.ArrangeBooks());
    }
}