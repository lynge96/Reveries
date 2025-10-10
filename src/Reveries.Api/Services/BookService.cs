using Reveries.Api.Interfaces;
using Reveries.Api.Mappers;
using Reveries.Application.Interfaces.Services;
using Reveries.Contracts.Books;
using Reveries.Core.Exceptions;

namespace Reveries.Api.Services;

public class BookService : IBookService
{
    private readonly IBookLookupService _bookLookupService;
    
    public BookService(IBookLookupService bookLookupService)
    {
        _bookLookupService = bookLookupService;
    }
    
    public async Task<BookDto?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var isbnList = new List<string> {isbn};
        
        var books = await _bookLookupService.FindBooksByIsbnAsync(isbnList, cancellationToken);

        if (books.Count == 0)
            throw new NotFoundException($"Book with ISBN '{isbn}' was not found.");

        return books.First().ToDto();
    }

    public async Task<IEnumerable<BookDto>> GetAllBooksAsync(CancellationToken cancellationToken = default)
    {
        var books = await _bookLookupService.GetAllBooksAsync(cancellationToken);
        
        if (books.Count == 0)
            throw new NotFoundException("No books were found.");
        
        return books.Select(book => book.ToDto());
    }
}