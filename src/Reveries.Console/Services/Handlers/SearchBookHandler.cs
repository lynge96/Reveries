using System.Net;
using Reveries.Application.Interfaces.Services;
using Reveries.Console.Extensions;
using Reveries.Console.Interfaces;
using Reveries.Core.Models;
using Spectre.Console;

namespace Reveries.Console.Services.Handlers;

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
            new TextPrompt<string>("Indtast ISBN eller bogtitel:".AsPrimary())
                .PromptStyle($"{ConsoleThemeExtensions.Secondary}"));

        try
        {
            Book? book = null;
            
            var elapsed = await AnsiConsole.Status()
                .StartAsync("Searching...\n".AsPrimary(), async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Default);
                    ctx.SpinnerStyle(ConsoleThemeExtensions.Secondary);

                    var timer = System.Diagnostics.Stopwatch.StartNew();
                    book = await _bookService.GetBookByIsbnAsync(isbn);
                    
                    return timer.ElapsedMilliseconds;
                });
            
            if (book != null)
            {
                AnsiConsole.MarkupLine($"Elapsed time: {elapsed} ms\n".Italic().AsSuccess());
                AnsiConsole.MarkupLine($"Fundet bog: {book.Title}".AsSuccess());
            }
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            AnsiConsole.MarkupLine($"No books with ISBN: {isbn.AsSecondary()} were found in the database.".AsWarning());
        }

    }
}