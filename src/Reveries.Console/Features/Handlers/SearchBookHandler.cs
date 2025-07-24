using Reveries.Application.Interfaces.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Core.Enums;
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
            .RunWithStatusAsync(async () => 
            {
                if (IsIsbnFormat(searchInput))
                {
                    return await _bookService.GetBooksByIsbnStringAsync(searchInput, cancellationToken);
                }
            
                return await _bookService.GetBooksByTitleAsync(searchInput, languageCode: null, BookFormat.PhysicalOnly, cancellationToken);
            });

        AnsiConsole.MarkupLine($"\nElapsed search time: {elapsedMs} ms".Italic().AsInfo());
        AnsiConsole.Write(bookResults.DisplayBooks());
    }

    private static bool IsIsbnFormat(string input)
    {
        var parts = input.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        
        return parts.All(part => 
            part.Length >= 10 && 
            part.Length <= 13 && 
            part.All(c => char.IsDigit(c) || c == '-' || c == 'X'));
    }
    
}
