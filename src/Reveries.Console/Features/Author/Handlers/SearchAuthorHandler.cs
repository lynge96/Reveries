using System.Net;
using Reveries.Application.Interfaces.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Console.Features.Console.Interfaces;
using Spectre.Console;

namespace Reveries.Console.Features.Author.Handlers;

public class SearchAuthorHandler : IMenuHandler
{
    public MenuChoice MenuChoice => MenuChoice.SearchAuthor;
    private readonly IAuthorService _authorService;

    public SearchAuthorHandler(IAuthorService authorService)
    {
        _authorService = authorService;
    }
    
    public async Task HandleAsync(CancellationToken cancellationToken = default)
    {
        var authorInput = ConsolePromptUtility.GetUserInput("Enter author name:");

        try
        {
            var (authors, elapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
                .RunWithStatusAsync(async () => await _authorService.GetAuthorsByNameAsync(authorInput));

            if (!authors.Any())
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
            
            AnsiConsole.MarkupLine($"Elapsed book search time: {bookSearchElapsedMs} ms".Italic().AsInfo());

            if (books.Count == 0)
            {
                AnsiConsole.MarkupLine($"No books found for author: {selectedAuthor.AsSecondary()}".AsWarning());
                return;
            }

            AnsiConsole.Write(books.DisplayBooks());
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            AnsiConsole.MarkupLine($"No authors found for: {authorInput.AsSecondary()}".AsWarning());
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"An error occurred: {ex.Message}".AsError());
        }
    }
}