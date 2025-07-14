using System.Net;
using Reveries.Application.Interfaces.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Features.Console.Interfaces;
using Spectre.Console;

namespace Reveries.Console.Features.Book.Handlers;

public class SearchBookHandler : IMenuHandler
{
    private readonly IBookService _bookService;

    public SearchBookHandler(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task HandleAsync()
    {
        // 9780804139021 9780593099322
        var isbn = AnsiConsole.Prompt(
            new TextPrompt<string>("Please, enter one or more ISBNs, separated by comma or space:".AsPrimary())
                .PromptStyle($"{ConsoleThemeExtensions.Secondary}"));

        try
        {
            var (result, elapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
                .RunWithStatusAsync(async () => await _bookService.GetBooksByIsbnStringAsync(isbn));

            if (result != null)
            {
                AnsiConsole.MarkupLine($"\nElapsed search time: {elapsedMs} ms".Italic().AsInfo());
                
                AnsiConsole.Write(result.DisplayBooks());
            }
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            var isbnDisplay = isbn.Contains(',') || isbn.Contains(' ') 
                ? "the provided ISBNs" 
                : $"ISBN: {isbn.AsSecondary()}";
            AnsiConsole.MarkupLine($"No books with {isbnDisplay} were found in the database.".AsWarning());
        }
        
    }
}
