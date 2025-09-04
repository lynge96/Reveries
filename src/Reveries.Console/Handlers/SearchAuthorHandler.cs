using Reveries.Application.Interfaces.Isbndb;
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
    private readonly IIsbndbAuthorService _isbndbAuthorService;
    private readonly IBookSelectionService _bookSelectionService;
    private readonly IBookDisplayService _bookDisplayService;

    public SearchAuthorHandler(IIsbndbAuthorService isbndbAuthorService, IBookSelectionService bookSelectionService, IBookDisplayService bookDisplayService)
    {
        _isbndbAuthorService = isbndbAuthorService;
        _bookSelectionService = bookSelectionService;
        _bookDisplayService = bookDisplayService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var authorInput = ConsolePromptUtility.GetUserInput("Enter author name:");

        var (authors, elapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(async () => await _isbndbAuthorService.GetAuthorsByNameAsync(authorInput, cancellationToken));

        if (authors.Count == 0)
        {
            AnsiConsole.MarkupLine($"No authors found for: {authorInput.AsSecondary()}".AsWarning());
            return;
        }

        AnsiConsole.MarkupLine($"\nElapsed search time: {elapsedMs} ms".Italic().AsInfo());
        
        var selectedAuthor = authors.Count == 1 
            ? authors[0] 
            : ConsolePromptUtility.ShowSelectionPrompt("Select an author to see their books:", authors);
        
        var (bookResults, bookSearchElapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(async () => await _isbndbAuthorService.GetBooksForAuthorAsync(selectedAuthor, cancellationToken));
        
        if (bookResults.Count == 0)
        {
            AnsiConsole.MarkupLine($"No books found for author: {selectedAuthor.AsSecondary()}".AsWarning());
            return;
        }
        
        var filteredBooks = _bookSelectionService.FilterBooksByLanguage(bookResults);
        
        AnsiConsole.MarkupLine($"Elapsed book search time: {bookSearchElapsedMs} ms".Italic().AsInfo());
        AnsiConsole.Write(_bookDisplayService.DisplayBooks(filteredBooks));
    }
}