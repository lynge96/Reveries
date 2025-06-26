using Reveries.Application.Extensions.Mappers;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.Models;

namespace Reveries.Application.Services;

public class BookService
{
    private readonly IIsbndbBookClient _bookClient;

    public BookService(IIsbndbBookClient bookClient)
    {
        _bookClient = bookClient;
    }

    public async Task<Book?> GetBookByIsbnAsync(string isbn)
    {
        // TODO: Tjek i Cache og DB før API
        var response = await _bookClient.GetBookByIsbnAsync(isbn);

        var bookDto = response?.Book;
        
        var book = bookDto?.ToBook();
        
        return book;
    }
}