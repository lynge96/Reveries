using Reveries.Application.Extensions.Mappers;
using Reveries.Application.Interfaces;
using Reveries.Core.Models;

namespace Reveries.Application.Services;

public class BookService
{
    private readonly IIsbndbClient _isbndb;

    public BookService(IIsbndbClient isbndb)
    {
        _isbndb = isbndb;
    }

    public async Task<Book?> GetBookByIsbnAsync(string isbn)
    {
        // TODO: Tjek i Cache og DB før API
        var bookDto = await _isbndb.GetBookByIsbnAsync(isbn);

        var book = bookDto?.ToBook();
        
        return book;
    }
}