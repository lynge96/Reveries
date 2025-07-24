using System.Net;
using Reveries.Application.Extensions;
using Reveries.Application.Interfaces.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Console.Features.Handlers.Interfaces;
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
            .RunWithStatusAsync(async () => 
            {
                var publishers = await _publisherService.GetPublishersByNameAsync(publisherInput);
                return publishers;
            });

        AnsiConsole.MarkupLine($"\nElapsed search time: {elapsedMs} ms".Italic().AsInfo());

        var uniquePublishers = PublisherNormalizer.GetUniquePublishers(publishers);
        
        var selectedPublisher = ConsolePromptUtility.ShowSelectionPrompt(
            "Select a publisher to see their books:",
            uniquePublishers);
        
        var (books, bookSearchElapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(async () => await _publisherService.GetBooksByPublisherAsync(selectedPublisher));
        
        AnsiConsole.MarkupLine($"Elapsed book search time: {bookSearchElapsedMs} ms".Italic().AsInfo());

        if (books.Count == 0)
        {
            AnsiConsole.MarkupLine($"No books found for publisher: {selectedPublisher.AsSecondary()}".AsWarning());
            return;
        }

        AnsiConsole.Write(books.DisplayBooks());
    }
}