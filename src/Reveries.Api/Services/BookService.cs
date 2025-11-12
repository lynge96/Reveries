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
    
    public async Task<BookDto?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Lookup book by ISBN started {@Ctx}", new { Operation = "GetBookByIsbn", Isbn = isbn });
        
        var books = await _bookLookupService.FindBooksByIsbnAsync([isbn], cancellationToken);

        if (books.Count == 0)
        {
            _logger.LogWarning("Lookup book by ISBN returned no results {@Ctx}", new { Operation = "GetBookByIsbn", Isbn = isbn });
            
            throw new NotFoundException($"Book with ISBN '{isbn}' was not found.");
        }

        var bookDto = books.First().ToDto();
        _logger.LogInformation("Lookup book by ISBN succeeded {@Ctx}", new { Operation = "GetBookByIsbn", Isbn = isbn });
        return bookDto;
    }

    public async Task<BookDto?> GetBookByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var book = await _bookLookupService.FindBookById(id, cancellationToken);
        
        if (book == null)
            throw new NotFoundException("No book was found with the given id.");
        
        var bookDto = book.ToDto();
        
        return bookDto;
    }

    public async Task<IEnumerable<BookDto>> GetAllBooksAsync(CancellationToken cancellationToken = default)
    {
        var books = await _bookLookupService.GetAllBooksAsync(cancellationToken);
        
        if (books.Count == 0)
            throw new NotFoundException("No books were found.");
        
        return books.Select(book => book.ToDto());
    }

    public async Task<int> CreateBookAsync(CreateBookDto bookDto, CancellationToken cancellationToken = default)
    {
        var book = bookDto.ToDomain();

        var bookId = await _bookManagementService.CreateBookWithRelationsAsync(book, cancellationToken);
        
        return bookId;
    }
}