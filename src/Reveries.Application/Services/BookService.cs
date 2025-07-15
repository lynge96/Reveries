using Reveries.Application.Common.Validation;
using Reveries.Application.Extensions.Mappers;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Models;

namespace Reveries.Application.Services;

public class BookService : IBookService
{
    private readonly IIsbndbBookClient _bookClient;

    public BookService(IIsbndbBookClient bookClient)
    {
        _bookClient = bookClient;
    }
    
    public async Task<BooksListResponse> GetBooksByIsbnStringAsync(string isbnString, CancellationToken cancellationToken = default)
    {
        var isbns = isbnString.Split([',', ' '], StringSplitOptions.RemoveEmptyEntries).ToList();

        if (isbns.Count == 1)
        {
            var book = await GetBookByIsbnAsync(isbns[0], cancellationToken);
            return book != null 
                ? new BooksListResponse(1, 1, new List<Book> { book }) 
                : new BooksListResponse(0, 1, new List<Book>());
        }
        
        var bookList = await GetBooksByIsbnsAsync(isbns, cancellationToken);
        
        return bookList!;
    }
    
    public async Task<List<Book>> GetBooksByTitleAsync(string title, string? languageCode, CancellationToken cancellationToken = default)
    {
        var response = await _bookClient.GetBooksByQueryAsync(title, languageCode, shouldMatchAll: true, cancellationToken);
        
        if (response?.Books is null)
            return new List<Book>();
        
        return response.Books
            .Select(bookDto => bookDto.ToBook())
            .ToList();
    }

    private async Task<Book?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var normalizedIsbn = IsbnValidationHelper.ValidateSingleIsbn(isbn);

        // TODO: Tjek i Cache og DB før API
        var response = await _bookClient.GetBookByIsbnAsync(normalizedIsbn, cancellationToken);

        var bookDto = response?.Book;
        
        var book = bookDto?.ToBook();
        
        return book;
    }
    
    private async Task<BooksListResponse?> GetBooksByIsbnsAsync(List<string> isbns, CancellationToken cancellationToken = default)
    {
        if (isbns.Count > 100)
            throw new ArgumentException("Too many ISBN numbers. Maximum is 100.");
        
        var validatedIsbns = IsbnValidationHelper.ValidateIsbns(isbns).ValidIsbns;

        var response = await _bookClient.GetBooksByIsbnsAsync(validatedIsbns, cancellationToken);
        
        if (response is null)
            return new BooksListResponse(0, 0, new List<Book>());

        var books = response.Data
            .Select(bookDto => bookDto.ToBook())
            .ToList();

        return new BooksListResponse(
            books.Count,
            validatedIsbns.Count,
            books);
    }
}