using Reveries.Application.Extensions.Mappers;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Validation;
using Reveries.Core.Models;

namespace Reveries.Application.Services;

public class BookService : IBookService
{
    private readonly IIsbndbBookClient _bookClient;

    public BookService(IIsbndbBookClient bookClient)
    {
        _bookClient = bookClient;
    }

    public async Task<Book?> GetBookByIsbnAsync(string isbn)
    {
        var normalizedIsbn = IsbnValidator.Normalize(isbn);
        if (!IsbnValidator.IsValid(normalizedIsbn))
            throw new ArgumentException("ISBN must be either 10 or 13 digits.", nameof(isbn));

        // TODO: Tjek i Cache og DB før API
        var response = await _bookClient.GetBookByIsbnAsync(normalizedIsbn);

        var bookDto = response?.Book;
        
        var book = bookDto?.ToBook();
        
        return book;
    }
}