using Reveries.Application.DTOs;
using Reveries.Application.Interfaces;

namespace Reveries.Application.Services;

public class BookService
{
    private readonly IIsbndbClient _isbndb;

    public BookService(IIsbndbClient isbndb)
    {
        _isbndb = isbndb;
    }

    public async Task<BookDto?> GetBookByIsbnAsync(string isbn)
    {
        var book = await _isbndb.GetBookByIsbnAsync(isbn);
        
        return book;
    }
}