using Reveries.Api.Interfaces;
using Reveries.Api.Mappers;
using Reveries.Application.Interfaces.Services;
using Reveries.Contracts.DTOs;
using Reveries.Core.Exceptions;

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
    
    public async Task<BookDto?> GetBookByIsbnAsync(string isbn, CancellationToken ct)
    {
        var books = await _bookLookupService.FindBooksByIsbnAsync([isbn], ct);

        if (books.Count == 0)
        {
            throw new NotFoundException($"Book with ISBN '{isbn}' was not found.");
        }

        var bookDto = books.First().ToDto();
        
        _logger.LogInformation("Lookup succeeded {@LookupContext}", new { Operation = "GetBookByIsbn", Isbn = isbn, bookDto.Title });
        return bookDto;
    }

    public async Task<IEnumerable<BookDto>> GetBooksByIsbnsAsync(List<string> isbns, CancellationToken ct)
    {
        var books = await _bookLookupService.FindBooksByIsbnAsync(isbns, ct);

        if (books.Count == 0)
        {
            throw new NotFoundException($"Books with ISBNs '{isbns}' were not found.");
        }
        
        var booksDto = books.Select(book => book.ToDto()).ToList();
        
        _logger.LogInformation("Lookup succeeded {@Ctx}", new { Operation = "GetBooksByIsbns", Titles = string.Join(", ", booksDto.Select(b => b.Title)) });
        return booksDto;
    }

    public async Task<BookDto?> GetBookByIdAsync(int id, CancellationToken ct)
    {
        var book = await _bookLookupService.FindBookById(id, ct);

        if (book == null)
        {
            throw new NotFoundException("No book was found with the given id.");
        }
        
        var bookDto = book.ToDto();
        
        _logger.LogInformation("Lookup succeeded {@Ctx}", new { Operation = "GetBookById", BookId = id, bookDto.Title });
        return bookDto;
    }

    public async Task<IEnumerable<BookDto>> GetAllBooksAsync(CancellationToken ct)
    {
        var books = await _bookLookupService.GetAllBooksAsync(ct);

        if (books.Count == 0)
        {
            throw new NotFoundException("No books were found.");
        }
        
        _logger.LogInformation("Lookup succeeded {@Ctx}", new { Operation = "GetAllBooks" });
        return books.Select(book => book.ToDto());
    }

    public async Task<int> CreateBookAsync(CreateBookDto bookDto, CancellationToken ct)
    {
        var book = bookDto.ToDomain();

        var bookId = await _bookManagementService.CreateBookWithRelationsAsync(book, ct);
        
        _logger.LogInformation("Book created successfully {@Ctx}", new { Operation = "CreateBook", BookId = bookId });
        return bookId;
    }
}