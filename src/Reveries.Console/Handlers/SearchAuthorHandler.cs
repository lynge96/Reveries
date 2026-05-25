using Reveries.Application.Authors.Services;
using Reveries.Application.Books.Extensions;
using Reveries.Application.Books.Queries.FindBooksByAuthor;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Console.Services;
using Reveries.Core.Helpers;
using Reveries.Core.Models;
using Spectre.Console;

namespace Reveries.Console.Handlers;

public class SearchAuthorHandler : BaseHandler
{
    public override MenuChoice MenuChoice => MenuChoice.SearchAuthor;
    private readonly BookSelectionService _bookSelectionService;
    private readonly BookDisplayService _bookDisplayService;
    private readonly AuthorLookupService _authorLookupService;
    private readonly FindBooksByAuthorHandler _findBooksByAuthorHandler;

    public SearchAuthorHandler(
        BookSelectionService bookSelectionService, 
        BookDisplayService bookDisplayService, 
        AuthorLookupService authorLookupService,
        FindBooksByAuthorHandler booksByAuthorHandler)
    {
        _bookSelectionService = bookSelectionService;
        _bookDisplayService = bookDisplayService;
        _authorLookupService = authorLookupService;
        _findBooksByAuthorHandler = booksByAuthorHandler;
    }
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var authorInput = ConsolePromptUtility.GetUserInput("Enter author name:");
        var author = Author.Create(authorInput);

        var (authors, elapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(async () => await _authorLookupService.FindAuthorsByNameAsync(author, ct));
        
        AnsiConsole.MarkupLine($"\nElapsed search time: {elapsedMs} ms".Italic().AsInfo());

        if (authors.Count == 0)
        {
            AnsiConsole.MarkupLine($"No authors found for: {authorInput.AsSecondary()}".AsWarning());
            return;
        }
        
        var selectedAuthor = authors.Count == 1 
            ? authors[0] 
            : ConsolePromptUtility.ShowSelectionPrompt("Select an author to see their books:", authors);

        var authorQuery = new FindBooksByAuthorQuery(selectedAuthor.NormalizedName);
        var (bookResults, bookSearchElapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(async () => await _findBooksByAuthorHandler.Handle(authorQuery, ct));
        
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