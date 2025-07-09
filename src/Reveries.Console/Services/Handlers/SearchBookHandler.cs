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
            new TextPrompt<string>("Please, enter the ISBN:".AsPrimary())
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
                AnsiConsole.MarkupLine($"Elapsed time: {elapsed} ms\n".Italic().AsInfo());
                AnsiConsole.MarkupLine($"Success! I found this book in the database:".AsSuccess());

                var table = new Table()
                    .Border(TableBorder.Markdown)
                    .BorderColor(Color.Tan)
                    .AddColumn(new TableColumn("Property".AsPrimary()))
                    .AddColumn(new TableColumn("Value".AsPrimary()))
                    .HideHeaders()
                    .Title($"📘 {book.ToString().Bold().AsPrimary()}");

                table.Columns[0].Alignment(Justify.Left);
                table.Columns[1].Alignment(Justify.Right);
                
                table.AddRow("Title", book.Title.AsSecondary());
                table.AddRow("Author", string.Join(", ", book.Authors).AsSecondary());
                table.AddRow("ISBN-10", book.Isbn10.AsSecondary());
                table.AddRow("ISBN-13", book.Isbn13.AsSecondary());
                table.AddRow("Publisher", book.Publisher.AsSecondary());
                table.AddRow("Pages", book.Pages.ToString().AsSecondary());
                table.AddRow("Language", book.Language.AsSecondary());
                table.AddRow("Published", book.PublishDate.ToString().AsSecondary());
                table.AddRow("MSRP", book.Msrp.ToString().AsSecondary());
                table.AddRow("Binding", book.Binding.AsSecondary());
                // table.AddRow("Subjects", string.Join(", ", book.Subjects).AsSecondary());
                // table.AddRow("Synopsis", book.Synopsis.AsSecondary());
    
                AnsiConsole.Write(table);
            }
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            AnsiConsole.MarkupLine($"No books with ISBN: {isbn.AsSecondary()} were found in the database.".AsWarning());
        }

    }
}