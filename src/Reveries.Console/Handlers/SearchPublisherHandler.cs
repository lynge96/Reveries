using Reveries.Application.Books.Extensions;
using Reveries.Application.Books.Queries.FindBooksByPublisher;
using Reveries.Application.Publishers.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Console.Services;
using Reveries.Core.Models;
using Spectre.Console;

namespace Reveries.Console.Handlers;

public class SearchPublisherHandler : BaseHandler
{
    public override MenuChoice MenuChoice => MenuChoice.SearchPublisher;
    private readonly BookSelectionService _bookSelectionService;
    private readonly BookDisplayService _bookDisplayService;
    private readonly PublisherLookupService _publisherLookupService;
    private readonly FindBooksByPublisherHandler _findBooksByPublisherHandler;

    public SearchPublisherHandler(
        BookSelectionService bookSelectionService, 
        BookDisplayService bookDisplayService, 
        PublisherLookupService publisherLookupService,
        FindBooksByPublisherHandler booksByPublisherHandler)
    {
        _bookSelectionService = bookSelectionService;
        _bookDisplayService = bookDisplayService;
        _publisherLookupService = publisherLookupService;
        _findBooksByPublisherHandler = booksByPublisherHandler;
    }
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var publisherInput = ConsolePromptUtility.GetUserInput("Enter publisher name:");
        var publisher = Publisher.Create(publisherInput);
        
        var (publishers, elapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(() => _publisherLookupService.FindPublishersByNameAsync(publisher, ct));

        AnsiConsole.MarkupLine($"\nElapsed search time: {elapsedMs} ms".Italic().AsInfo());
        
        var selectedPublisher = ConsolePromptUtility.ShowSelectionPrompt("Select a publisher to see their books:", publishers);
        
        var publisherQuery = new FindBooksByPublisherQuery(selectedPublisher);
        var (bookResults, bookSearchElapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(async () => await _findBooksByPublisherHandler.Handle(publisherQuery, ct));
        
        AnsiConsole.MarkupLine($"Elapsed book search time: {bookSearchElapsedMs} ms".Italic().AsInfo());
        
        if (bookResults.Count == 0)
        {
            AnsiConsole.MarkupLine(
                $"No books found for publisher: {selectedPublisher.Name.AsSecondary()}".AsWarning());
            return;
        }

        var filteredBooks = _bookSelectionService.FilterBooksByLanguage(bookResults);
        
        _bookDisplayService.DisplayBooksTable(filteredBooks.ArrangeBooks());
    }
}