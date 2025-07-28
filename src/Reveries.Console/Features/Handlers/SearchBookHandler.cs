using Reveries.Application.Interfaces.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Core.Enums;
using Reveries.Core.Models;
using Spectre.Console;

namespace Reveries.Console.Features.Handlers;

public class SearchBookHandler : BaseHandler
{
    public override MenuChoice MenuChoice => MenuChoice.SearchBook;
    private readonly IBookService _bookService;

    public SearchBookHandler(IBookService bookService)
    {
        _bookService = bookService;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // 9780804139021 9780593099322
        // 9788799338238
        var searchInput = ConsolePromptUtility.GetUserInput("Enter book title or ISBN, separated by comma or space:");
        
        var (bookResults, elapsedMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(() => SearchBooksAsync(searchInput, cancellationToken));

        var filteredBooks = bookResults.SelectLanguages();
        
        AnsiConsole.MarkupLine($"\nElapsed search time: {elapsedMs} ms".Italic().AsInfo());
        AnsiConsole.Write(filteredBooks.DisplayBooks());
    }

    private Task<List<Book>> SearchBooksAsync(string searchInput, CancellationToken cancellationToken)
    {
        return IsIsbnFormat(searchInput)
            ? _bookService.GetBooksByIsbnStringAsync(searchInput, cancellationToken)
            : _bookService.GetBooksByTitleAsync(searchInput, languageCode: null, BookFormat.PhysicalOnly, cancellationToken);
    }

    private static bool IsIsbnFormat(string input)
    {
        var parts = input.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        
        const int isbn10Length = 10;
        const int isbn13Length = 13;
        
        return parts.All(part => 
            part.Length is >= isbn10Length and <= isbn13Length && 
            part.All(c => char.IsDigit(c) || c == '-' || c == 'X'));
    }
    
}
