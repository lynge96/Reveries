using Reveries.Application.Extensions;
using Reveries.Application.Interfaces.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Console.Services.Interfaces;
using Spectre.Console;

namespace Reveries.Console.Handlers;

public class SearchAuthorHandler : BaseHandler
{
    public override MenuChoice MenuChoice => MenuChoice.SearchAuthor;
    private readonly IBookSelectionService _bookSelectionService;
    private readonly IBookDisplayService _bookDisplayService;
    private readonly IBookLookupService _bookLookupService;
    private readonly IAuthorLookupService _authorLookupService;

    public SearchAuthorHandler(IBookLookupService bookLookupService, IBookSelectionService bookSelectionService, IBookDisplayService bookDisplayService, IAuthorLookupService authorLookupService)
    {
        _bookSelectionService = bookSelectionService;
        _bookDisplayService = bookDisplayService;
        _bookLookupService = bookLookupService;
        _authorLookupService = authorLookupService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var authorInput = ConsolePromptUtility.GetUserInput("Enter author name:");

        var (authors, elapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(async () => await _authorLookupService.FindAuthorsByNameAsync(authorInput, cancellationToken));
        
        AnsiConsole.MarkupLine($"\nElapsed search time: {elapsedMs} ms".Italic().AsInfo());

        if (authors.Count == 0)
        {
            AnsiConsole.MarkupLine($"No authors found for: {authorInput.AsSecondary()}".AsWarning());
            return;
        }
        
        var selectedAuthor = authors.Count == 1 
            ? authors[0] 
            : ConsolePromptUtility.ShowSelectionPrompt("Select an author to see their books:", authors);
        
        var (bookResults, bookSearchElapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(async () => await _bookLookupService.FindBooksByAuthorAsync(selectedAuthor.NormalizedName, cancellationToken));
        
        AnsiConsole.MarkupLine($"Elapsed book search time: {bookSearchElapsedMs} ms".Italic().AsInfo());
        
        if (bookResults.Count == 0)
        {
            AnsiConsole.MarkupLine($"No books found for author: {selectedAuthor.NormalizedName.ToTitleCase().AsSecondary()}".AsWarning());
            return;
        }
        
        var filteredBooks = _bookSelectionService.FilterBooksByLanguage(bookResults);
        
        _bookDisplayService.DisplayBooksTable(filteredBooks.ArrangeBooks());
    }
}