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
        _logger.LogInformation("Lookup book by ISBN started {@LookupContext}", new { Operation = "GetBookByIsbn", Isbn = isbn });
        
        var books = await _bookLookupService.FindBooksByIsbnAsync([isbn], ct);

        if (books.Count == 0)
        {
            _logger.LogWarning("Lookup book by ISBN returned no results: {@Isbn}", isbn);
            
            throw new NotFoundException($"Book with ISBN '{isbn}' was not found.");
        }

        var bookDto = books.First().ToDto();
        
        _logger.LogInformation("Lookup succeeded {@LookupContext}", new { Operation = "GetBookByIsbn", Isbn = isbn, bookDto.Title });
        return bookDto;
    }

    public async Task<IEnumerable<BookDto>> GetBooksByIsbnsAsync(List<string> isbns, CancellationToken ct)
    {
        _logger.LogInformation("Lookup books by ISBNs started {@Ctx}", new { Operation = "GetBooksByIsbns", Isbns = isbns });
        
        var books = await _bookLookupService.FindBooksByIsbnAsync(isbns, ct);

        if (books.Count == 0)
        {
            _logger.LogWarning("Lookup books by ISBNs returned no results: {@Isbns}", isbns);
            
            throw new NotFoundException($"Books with ISBNs '{isbns}' were not found.");
        }
        
        var booksDto = books.Select(book => book.ToDto()).ToList();
        
        _logger.LogInformation("Lookup succeeded {@Ctx}", new { Operation = "GetBooksByIsbns", Titles = string.Join(", ", booksDto.Select(b => b.Title)) });
        return booksDto;
    }

    public async Task<BookDto?> GetBookByIdAsync(int id, CancellationToken ct)
    {
        _logger.LogInformation("Lookup book by Id started {@Ctx}", new { Operation = "GetBookById", BookId = id });
        
        var book = await _bookLookupService.FindBookById(id, ct);

        if (book == null)
        {
            _logger.LogWarning("Book with Id: {BookId} was not found in the database", id);
            
            throw new NotFoundException("No book was found with the given id.");
        }
        
        var bookDto = book.ToDto();
        
        _logger.LogInformation("Lookup succeeded {@Ctx}", new { Operation = "GetBookById", BookDto = bookDto });
        return bookDto;
    }

    public async Task<IEnumerable<BookDto>> GetAllBooksAsync(CancellationToken ct)
    {
        _logger.LogInformation("Lookup all books started {@Ctx}", new { Operation = "GetAllBooks" });
        
        var books = await _bookLookupService.GetAllBooksAsync(ct);

        if (books.Count == 0)
        {
            _logger.LogWarning("No books were found in the database");
            
            throw new NotFoundException("No books were found.");
        }
        
        _logger.LogInformation("Lookup succeeded {@Ctx}", new { Operation = "GetAllBooks" });
        return books.Select(book => book.ToDto());
    }

    public async Task<int> CreateBookAsync(CreateBookDto bookDto, CancellationToken ct)
    {
        _logger.LogInformation("Creating book started {@Ctx}", new { Operation = "CreateBook", BookDto = bookDto });
        
        var book = bookDto.ToDomain();

        var bookId = await _bookManagementService.CreateBookWithRelationsAsync(book, ct);
        
        _logger.LogInformation("Book created successfully {@Ctx}", new { Operation = "CreateBook", BookId = bookId });
        return bookId;
    }
}