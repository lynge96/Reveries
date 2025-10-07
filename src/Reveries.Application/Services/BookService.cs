using Reveries.Application.Common.Exceptions;
using Reveries.Application.Common.Mappers;
using Reveries.Application.Interfaces.Services;
using Reveries.Contracts.Books;

namespace Reveries.Application.Services;

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

        var bookDto = books.First().ToDto();

        return bookDto;
    }
}