using Reveries.Application.Common.Mappers;
using Reveries.Application.Extensions;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.Entities;
using Reveries.Core.Enums;

namespace Reveries.Application.Services.Isbndb;

public class IsbndbBookService : IIsbndbBookService
{
    private readonly IIsbndbBookClient _bookClient;

    public IsbndbBookService(IIsbndbBookClient bookClient)
    {
        _bookClient = bookClient;
    }
    
    public async Task<List<Book>> GetBooksByIsbnsAsync(List<string> isbns, CancellationToken cancellationToken = default)
    {
        var isbndbBooks = new List<Book>();

        switch (isbns.Count)
        {
            case 1:
            {
                var book = await GetSingleBookAsync(isbns[0], cancellationToken);
                if (book != null)
                    isbndbBooks.Add(book);
                break;
            }
            case > 1:
                isbndbBooks = await GetMultipleBooksAsync(isbns, cancellationToken);
                break;
        }

        return isbndbBooks;
    }
    
    public async Task<List<Book>> GetBooksByTitlesAsync(List<string> titles, string? languageCode, CancellationToken cancellationToken = default)
    {
        var booksFromApi = new List<Book>();
        
        foreach (var title in titles)
        {
            var response = await _bookClient.SearchBooksByQueryAsync(title, languageCode, shouldMatchAll: true, cancellationToken);
            if (response?.Books != null)
            {
                booksFromApi.AddRange(response.Books.Select(b => b.ToBook()));
            }
        }
        
        return booksFromApi
            .FilterByFormat(BookFormat.PhysicalOnly)
            .ToList();
    }

    private async Task<Book?> GetSingleBookAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var response = await _bookClient.FetchBookByIsbnAsync(isbn, cancellationToken);

        var bookDto = response?.Book;
        
        var book = bookDto?.ToBook();
        
        return book;
    }
    
    private async Task<List<Book>> GetMultipleBooksAsync(List<string> isbns, CancellationToken cancellationToken = default)
    {
        if (isbns.Count > 100)
            throw new ArgumentException("Too many ISBN numbers. Maximum is 100.");

        var response = await _bookClient.FetchBooksByIsbnsAsync(isbns, cancellationToken);
        
        if (response is null)
            return new List<Book>();
        
        return response.Data
            .Select(bookDto => bookDto.ToBook())
            .ToList();
    }
}