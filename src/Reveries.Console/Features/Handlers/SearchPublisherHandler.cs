using Reveries.Application.Extensions;
using Reveries.Application.Interfaces.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Spectre.Console;

namespace Reveries.Console.Features.Handlers;

public class SearchPublisherHandler : BaseHandler
{
    public override MenuChoice MenuChoice => MenuChoice.SearchPublisher;
    private readonly IPublisherService _publisherService;

    public SearchPublisherHandler(IPublisherService publisherService)
    {
        _publisherService = publisherService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var publisherInput = ConsolePromptUtility.GetUserInput("Enter publisher name:");
        
        var (publishers, elapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(() => _publisherService.GetPublishersByNameAsync(publisherInput, cancellationToken));

        AnsiConsole.MarkupLine($"\nElapsed search time: {elapsedMs} ms".Italic().AsInfo());

        var uniquePublishers = PublisherNormalizer.GetUniquePublishers(publishers);
        
        var selectedPublisher = ConsolePromptUtility.ShowSelectionPrompt(
            "Select a publisher to see their books:",
            uniquePublishers);
        
        var (books, bookSearchElapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(async () => await _publisherService.GetBooksByPublisherAsync(selectedPublisher));
        
        if (books.Count == 0)
        {
            AnsiConsole.MarkupLine($"No books found for publisher: {selectedPublisher.AsSecondary()}".AsWarning());
            return;
        }

        var filteredBooks = books.SelectLanguages();
        
        AnsiConsole.MarkupLine($"Elapsed book search time: {bookSearchElapsedMs} ms".Italic().AsInfo());
        AnsiConsole.Write(filteredBooks.DisplayBooks());
    }
}