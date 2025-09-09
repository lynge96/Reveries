using Reveries.Application.Interfaces.Isbndb;
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
    private readonly IIsbndbBookService _isbndbBookService;
    private readonly IBookSaveService _bookSaveService;
    private readonly IBookSelectionService _bookSelectionService;
    private readonly IBookDisplayService _bookDisplayService;
    private readonly IBookEnrichmentService _bookEnrichmentService;

    public SearchBookHandler(IIsbndbBookService isbndbBookService, IBookSaveService bookSaveService, IBookSelectionService bookSelectionService, IBookDisplayService bookDisplayService, IBookEnrichmentService bookEnrichmentService)
    {
        _isbndbBookService = isbndbBookService;
        _bookSaveService = bookSaveService;
        _bookSelectionService = bookSelectionService;
        _bookDisplayService = bookDisplayService;
        _bookEnrichmentService = bookEnrichmentService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // 9780804139021 9780593099322 9781982141172
        // 9781399725026
        var searchInput = ConsolePromptUtility.GetUserInput("Enter book title or ISBN, separated by comma or space:");

        var (bookResults, elapsedSearchMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(() => SearchBooksAsync(searchInput, cancellationToken));

        var filteredBooks = _bookSelectionService.FilterBooksByLanguage(bookResults);
        
        AnsiConsole.MarkupLine($"\nElapsed search time: {elapsedSearchMs} ms".Italic().AsInfo());
        AnsiConsole.Write(_bookDisplayService.DisplayBooks(filteredBooks));

        var booksToSave = _bookSelectionService.SelectBooksToSave(filteredBooks);
        
        if (booksToSave.Count > 0)
            await _bookSaveService.SaveBooksAsync(booksToSave, cancellationToken);
    }

    private async Task<List<Book>> SearchBooksAsync(string searchInput, CancellationToken cancellationToken)
    {
        var tokens = searchInput
            .Split([',', ' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        var isbnTokens = tokens.Where(IsIsbnFormat).ToList();
        var titleTokens = tokens.Except(isbnTokens).ToList();

        var results = new List<Book>();

        if (isbnTokens.Count != 0)
        {
            var isbn = await _bookEnrichmentService.EnrichBooksByIsbnsAsync(isbnTokens, cancellationToken);
            //var isbnResults = await _isbndbBookService.GetBooksByIsbnStringAsync(isbnTokens, cancellationToken);
            //results.AddRange(isbnResults);
        }

        if (titleTokens.Count != 0)
        {
            var titleResults = await _isbndbBookService.GetBooksByTitleAsync(titleTokens, languageCode: null, BookFormat.PhysicalOnly, cancellationToken);
            results.AddRange(titleResults);
        }

        return results;
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
