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
    
    public async Task<BooksListResponse?> GetBooksByIsbnStringAsync(string isbnString)
    {
        var isbns = isbnString.Split([',', ' '], StringSplitOptions.RemoveEmptyEntries).ToList();

        if (isbns.Count == 1)
        {
            var book = await GetBookByIsbnAsync(isbns[0]);
            return book != null 
                ? new BooksListResponse(1, 1, new List<Book> { book }) 
                : new BooksListResponse(0, 1, new List<Book>());
        }
        
        var bookList = await GetBooksByIsbnsAsync(isbns);
        
        return bookList;
    }


    public async Task<List<Book?>> GetBookByTitleAsync(string title, string? languageCode)
    {
        throw new NotImplementedException();
    }

    private async Task<Book?> GetBookByIsbnAsync(string isbn)
    {
        var normalizedIsbn = IsbnValidationHelper.ValidateSingleIsbn(isbn);

        // TODO: Tjek i Cache og DB før API
        var response = await _bookClient.GetBookByIsbnAsync(normalizedIsbn);

        var bookDto = response?.Book;
        
        var book = bookDto?.ToBook();
        
        return book;
    }
    
    private async Task<BooksListResponse?> GetBooksByIsbnsAsync(List<string> isbns)
    {
        if (isbns.Count > 100)
            throw new ArgumentException("Too many ISBN numbers. Maximum is 100.");
        
        var validatedIsbns = IsbnValidationHelper.ValidateIsbns(isbns).ValidIsbns;

        var response = await _bookClient.GetBooksByIsbnsAsync(validatedIsbns);
        
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