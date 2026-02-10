using Reveries.Api.Interfaces;
using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Mappers;
using Reveries.Application.Queries;
using Reveries.Contracts.Books;
using Reveries.Core.Exceptions;
using Reveries.Core.Identity;
using Reveries.Core.ValueObjects;

namespace Reveries.Api.Services;

public class BookService : IBookService
{
    private readonly IBookLookupService _bookLookupService;
    private readonly IBookManagementService _bookManagementService;
    private readonly ILogger<BookService> _logger;
    
    public BookService(IBookLookupService bookLookupService, IBookManagementService bookManagementService, ILogger<BookService> logger)
    {
        _bookLookupService = bookLookupService;
        _bookManagementService = bookManagementService;
        _logger = logger;
    }

    public async Task<IEnumerable<BookDetailsReadModel>> GetBooksByIsbnsAsync(List<Isbn> isbns, CancellationToken ct)
    {
        var books = await _bookLookupService.FindBooksByIsbnAsync(isbns, ct);

        if (books.Count == 0)
        {
            throw new NotFoundException($"Books with ISBNs '{isbns}' were not found.");
        }
        
        var booksDto = books.Select(book => book.ToReadModel()).ToList();
        
        _logger.LogInformation("Lookup succeeded {@Ctx}", new { Operation = "GetBooksByIsbns", Titles = string.Join(", ", booksDto.Select(b => b.Title)) });
        return booksDto;
    }

    public async Task<BookDetailsReadModel?> GetBookByIdAsync(int id, CancellationToken ct)
    {
        var book = await _bookLookupService.FindBookById(id, ct);

        if (book == null)
        {
            throw new NotFoundException("No book was found with the given id.");
        }
        
        var bookDto = book.ToReadModel();
        
        _logger.LogInformation("Lookup succeeded {@Ctx}", new { Operation = "GetBookById", BookId = id, bookDto.Title });
        return bookDto;
    }

    public async Task<IEnumerable<BookDetailsReadModel>> GetAllBooksAsync(CancellationToken ct)
    {
        var books = await _bookLookupService.GetAllBooksAsync(ct);

        if (books.Count == 0)
        {
            throw new NotFoundException("No books were found.");
        }
        
        _logger.LogInformation("Lookup succeeded {@Ctx}", new { Operation = "GetAllBooks" });
        return books.Select(book => book.ToReadModel());
    }

}