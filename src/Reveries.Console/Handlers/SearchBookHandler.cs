using Reveries.Application.Books.Extensions;
using Reveries.Application.Books.Queries.FindBooksByIsbns;
using Reveries.Application.Books.Queries.FindBooksByTitles;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Console.Services;
using Reveries.Core.Models;
using Spectre.Console;

namespace Reveries.Console.Handlers;

public class SearchBookHandler : BaseHandler
{
    public override MenuChoice MenuChoice => MenuChoice.SearchBook;
    private readonly SaveEntityService _saveEntityService;
    private readonly BookSelectionService _bookSelectionService;
    private readonly BookDisplayService _bookDisplayService;
    private readonly FindBooksByIsbnsHandler _booksByIsbnsHandler;
    private readonly FindBooksByTitlesHandler _booksByTitlesHandler;

    public SearchBookHandler(
        SaveEntityService saveEntityService, 
        BookSelectionService bookSelectionService, 
        BookDisplayService bookDisplayService, 
        FindBooksByIsbnsHandler booksByIsbnsHandler,
        FindBooksByTitlesHandler booksByTitlesHandler)
    {
        _saveEntityService = saveEntityService;
        _bookSelectionService = bookSelectionService;
        _bookDisplayService = bookDisplayService;
        _booksByIsbnsHandler = booksByIsbnsHandler;
        _booksByTitlesHandler = booksByTitlesHandler;
    }
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var searchInput = ConsolePromptUtility.GetUserInput("Enter book title or ISBN, separated by comma:");

        var (bookResults, elapsedSearchMs) = await AnsiConsole.Create(new AnsiConsoleSettings())
            .RunWithStatusAsync(() => SearchBooksAsync(searchInput, ct));

        AnsiConsole.MarkupLine($"\nElapsed search time: {elapsedSearchMs} ms".Italic().AsInfo());
        
        var filteredBooks = _bookSelectionService.FilterBooksByLanguage(bookResults);
        
        _bookDisplayService.DisplayBooksTable(filteredBooks.ArrangeBooks());

        var booksToSave = _bookSelectionService.SelectBooksToSave(filteredBooks);
        
        if (booksToSave.Count > 0)
            await _saveEntityService.SaveBooksAsync(booksToSave, ct);
    }

    private async Task<List<Book>> SearchBooksAsync(string searchInput, CancellationToken ct)
    {
        var tokens = searchInput
            .Split([','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        var isbnTokens = tokens.Where(IsIsbnFormat).ToList();
        var titleTokens = tokens.Except(isbnTokens).ToList();

        var results = new List<Book>();

        if (isbnTokens.Count != 0)
        {
            var query = new FindBooksByIsbnsQuery(isbnTokens);
            var books = await _booksByIsbnsHandler.Handle(query, ct);
            
            results.AddRange(books);
        }
        if (titleTokens.Count != 0)
        {
            var query = new FindBooksByTitlesQuery(titleTokens);
            var books = await _booksByTitlesHandler.Handle(query, ct);
            
            results.AddRange(books);
        }

        return results;
    }

    private static bool IsIsbnFormat(string input)
    {
        var parts = input.Split([','], StringSplitOptions.RemoveEmptyEntries);
        
        const int isbn10Length = 10;
        const int isbn13Length = 13;
        
        return parts.All(part => 
            part.Length is >= isbn10Length and <= isbn13Length && 
            part.All(c => char.IsDigit(c) || c == '-' || c == 'X'));
    }
    
}
