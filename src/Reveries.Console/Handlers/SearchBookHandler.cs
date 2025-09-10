using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Console.Services.Interfaces;
using Reveries.Core.Entities;
using Spectre.Console;

namespace Reveries.Console.Handlers;

public class SearchBookHandler : BaseHandler
{
    public override MenuChoice MenuChoice => MenuChoice.SearchBook;
    private readonly IBookSaveService _bookSaveService;
    private readonly IBookSelectionService _bookSelectionService;
    private readonly IBookDisplayService _bookDisplayService;
    private readonly IBookLookupService _bookLookupService;

    public SearchBookHandler(IBookSaveService bookSaveService, IBookSelectionService bookSelectionService, IBookDisplayService bookDisplayService, IBookLookupService bookLookupService)
    {
        _bookSaveService = bookSaveService;
        _bookSelectionService = bookSelectionService;
        _bookDisplayService = bookDisplayService;
        _bookLookupService = bookLookupService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var searchInput = ConsolePromptUtility.GetUserInput("Enter book title or ISBN, separated by comma:");

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
            .Split([','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        var isbnTokens = tokens.Where(IsIsbnFormat).ToList();
        var titleTokens = tokens.Except(isbnTokens).ToList();

        var results = new List<Book>();

        if (isbnTokens.Count != 0)
        {
            var books = await _bookLookupService.FindBooksByIsbnAsync(isbnTokens, cancellationToken);
            results.AddRange(books);
        }
        if (titleTokens.Count != 0)
        {
            var books = await _bookLookupService.FindBooksByTitleAsync(titleTokens, cancellationToken);
            results.AddRange(books);
        }

        return results;
    }

    private static bool IsIsbnFormat(string input)
    {
        var parts = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        
        const int isbn10Length = 10;
        const int isbn13Length = 13;
        
        return parts.All(part => 
            part.Length is >= isbn10Length and <= isbn13Length && 
            part.All(c => char.IsDigit(c) || c == '-' || c == 'X'));
    }
    
}
