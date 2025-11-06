using Reveries.Api.Interfaces;
using Reveries.Api.Mappers;
using Reveries.Application.Interfaces.Services;
using Reveries.Contracts.DTOs;
using Reveries.Core.Exceptions;
using Reveries.Core.Models;

namespace Reveries.Api.Services;

public class BookService : IBookService
{
    private readonly IBookLookupService _bookLookupService;
    private readonly IBookManagementService _bookManagementService;
    
    public BookService(IBookLookupService bookLookupService, IBookManagementService bookManagementService)
    {
        _bookLookupService = bookLookupService;
        _bookManagementService = bookManagementService;
    }
    
    public async Task<BookDto?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var isbnList = new List<string> {isbn};
        
        var books = await _bookLookupService.FindBooksByIsbnAsync(isbnList, cancellationToken);

        if (books.Count == 0)
            throw new NotFoundException($"Book with ISBN '{isbn}' was not found.");

        return books.First().ToDto();
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

    public async Task<int> CreateBookAsync(BookDto bookDto, CancellationToken cancellationToken = default)
    {
        var book = bookDto.ToDomain();

        var bookId = await _bookManagementService.CreateBookWithRelationsAsync(book, cancellationToken);
        
        return bookId;
    }
}