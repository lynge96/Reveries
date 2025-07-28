using Reveries.Application.Interfaces.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Spectre.Console;

namespace Reveries.Console.Features.Handlers;

public class SearchAuthorHandler : BaseHandler
{
    public override MenuChoice MenuChoice => MenuChoice.SearchAuthor;
    private readonly IAuthorService _authorService;

    public SearchAuthorHandler(IAuthorService authorService)
    {
        _authorService = authorService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var authorInput = ConsolePromptUtility.GetUserInput("Enter author name:");

        var (authors, elapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(async () => await _authorService.GetAuthorsByNameAsync(authorInput, cancellationToken));

        if (authors.Count == 0)
        {
            AnsiConsole.MarkupLine($"No authors found for: {authorInput.AsSecondary()}".AsWarning());
            return;
        }

        AnsiConsole.MarkupLine($"\nElapsed search time: {elapsedMs} ms".Italic().AsInfo());
        
        var selectedAuthor = ConsolePromptUtility.ShowSelectionPrompt(
            "Select an author to see their books:",
            authors);
        
        var (books, bookSearchElapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(async () => await _authorService.GetBooksForAuthorAsync(selectedAuthor));
        
        if (books.Count == 0)
        {
            AnsiConsole.MarkupLine($"No books found for author: {selectedAuthor.AsSecondary()}".AsWarning());
            return;
        }
        
        var filteredBooks = books.SelectLanguages();
        
        AnsiConsole.MarkupLine($"Elapsed book search time: {bookSearchElapsedMs} ms".Italic().AsInfo());
        AnsiConsole.Write(filteredBooks.DisplayBooks());
    }
}