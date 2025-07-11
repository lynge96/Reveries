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
        // 9780804139021
        var isbn = AnsiConsole.Prompt(
            new TextPrompt<string>("Please, enter the ISBN:".AsPrimary())
                .PromptStyle($"{ConsoleThemeExtensions.Secondary}"));

        try
        {
            var (result, elapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
                .RunWithStatusAsync(async () => await _bookService.GetBookByIsbnAsync(isbn));

            if (result != null)
            {
                AnsiConsole.MarkupLine($"\nElapsed search time: {elapsedMs} ms".Italic().AsInfo());

                AnsiConsole.Write(result.DisplayBook());
            }
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            AnsiConsole.MarkupLine($"No books with ISBN: {isbn.AsSecondary()} were found in the database.".AsWarning());
        }

    }
}
