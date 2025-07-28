using Reveries.Application.Common.Validation;
using Reveries.Application.Extensions;
using Reveries.Application.Extensions.Mappers;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Enums;
using Reveries.Core.Models;

namespace Reveries.Application.Services;

public class BookService : IBookService
{
    private readonly IIsbndbBookClient _bookClient;

    public BookService(IIsbndbBookClient bookClient)
    {
        _bookClient = bookClient;
    }
    
    public async Task<List<Book>> GetBooksByIsbnStringAsync(string isbnString, CancellationToken cancellationToken = default)
    {
        var isbns = isbnString.Split([',', ' '], StringSplitOptions.RemoveEmptyEntries).ToList();

        if (isbns.Count == 1)
        {
            var book = await GetBookByIsbnAsync(isbns[0], cancellationToken);
            return book != null 
                ? new List<Book> { book }
                : new List<Book>();
        }
    
        var bookList = await GetBooksByIsbnsAsync(isbns, cancellationToken);

        return bookList;
    }
    
    public async Task<List<Book>> GetBooksByTitleAsync(string title, string? languageCode, BookFormat format, CancellationToken cancellationToken = default)
    {
        var response = await _bookClient.GetBooksByQueryAsync(title, languageCode, shouldMatchAll: true, cancellationToken);
        
        if (response?.Books is null)
            return new List<Book>();
        
        return response.Books
            .Select(bookdto => bookdto.ToBook())
            // .Where(book => !string.IsNullOrWhiteSpace(book.Language) && !book.Language.Equals("unknown", StringComparison.InvariantCultureIgnoreCase))
            .FilterByFormat(format)
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
    
    private async Task<List<Book>> GetBooksByIsbnsAsync(List<string> isbns, CancellationToken cancellationToken = default)
    {
        if (isbns.Count > 100)
            throw new ArgumentException("Too many ISBN numbers. Maximum is 100.");
        
        var validatedIsbns = IsbnValidationHelper.ValidateIsbns(isbns).ValidIsbns;

        var response = await _bookClient.GetBooksByIsbnsAsync(validatedIsbns, cancellationToken);
        
        if (response is null)
            return new List<Book>();
        
        return response.Data
            .Select(bookDto => bookDto.ToBook())
            .ToList();
    }
}