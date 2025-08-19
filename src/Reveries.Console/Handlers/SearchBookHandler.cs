using Reveries.Application.Interfaces.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Console.Services.Interfaces;
using Reveries.Core.Entities;
using Reveries.Core.Enums;
using Spectre.Console;

namespace Reveries.Console.Handlers;

public class SearchBookHandler : BaseHandler
{
    public override MenuChoice MenuChoice => MenuChoice.SearchBook;
    private readonly IBookService _bookService;
    private readonly IBookSaveService _bookSaveService;
    private readonly IBookSelectionService _bookSelectionService;
    private readonly IBookDisplayService _bookDisplayService;

    public SearchBookHandler(IBookService bookService, IBookSaveService bookSaveService, IBookSelectionService bookSelectionService, IBookDisplayService bookDisplayService)
    {
        _bookService = bookService;
        _bookSaveService = bookSaveService;
        _bookSelectionService = bookSelectionService;
        _bookDisplayService = bookDisplayService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // 9780804139021 9780593099322
        // 9788799338238
        var searchInput = ConsolePromptUtility.GetUserInput("Enter book title or ISBN, separated by comma or space:");

        var (bookResults, elapsedSearchMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(() => SearchBooksAsync(searchInput, cancellationToken));

        var filteredBooks = _bookSelectionService.FilterBooksByLanguage(bookResults);
        
        AnsiConsole.MarkupLine($"\nElapsed search time: {elapsedSearchMs} ms".Italic().AsInfo());
        AnsiConsole.Write(_bookDisplayService.DisplayBooks(filteredBooks));

        var booksToSave = _bookSelectionService.SelectBooksToSave(filteredBooks);
        
        await _bookSaveService.SaveBooksAsync(booksToSave, cancellationToken);

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
