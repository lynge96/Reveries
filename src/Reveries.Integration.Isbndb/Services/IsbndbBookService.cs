using Microsoft.Extensions.Options;
using Reveries.Application.Extensions;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.Enums;
using Reveries.Core.Exceptions;
using Reveries.Core.Models;
using Reveries.Integration.Isbndb.Configuration;
using Reveries.Integration.Isbndb.Interfaces;
using Reveries.Integration.Isbndb.Mappers;

namespace Reveries.Integration.Isbndb.Services;

public class IsbndbBookService : IIsbndbBookService
{
    private readonly IIsbndbBookClient _bookClient;
    private readonly IsbndbSettings _settings;

    public IsbndbBookService(IIsbndbBookClient bookClient, IOptions<IsbndbSettings> options)
    {
        _bookClient = bookClient;
        _settings = options.Value;
    }
    
    public async Task<List<Book>> GetBooksByIsbnsAsync(List<string> isbns, CancellationToken cancellationToken = default)
    {
        var isbndbBooks = new List<Book>();

        switch (isbns.Count)
        {
            case 1:
            {
                var book = await GetSingleBookAsync(isbns[0], cancellationToken);
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
            var response = await _bookClient.SearchBooksAsync(title, languageCode, shouldMatchAll: true, ct: cancellationToken);

            booksFromApi.AddRange(response.Books.Select(b => b.ToBook()));
        }
        
        return booksFromApi
            .FilterByFormat(BookFormat.PhysicalOnly)
            .ToList();
    }

    private async Task<Book> GetSingleBookAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var response = await _bookClient.FetchBookByIsbnAsync(isbn, cancellationToken);

        var bookDto = response.Book;

        return bookDto.ToBook();
    }
    
    private async Task<List<Book>> GetMultipleBooksAsync(List<string> isbns, CancellationToken cancellationToken = default)
    {
        if (isbns.Count > _settings.MaxBulkIsbns)
            throw new IsbnValidationException($"Too many ISBN numbers. Maximum is {_settings.MaxBulkIsbns}.");

        var response = await _bookClient.FetchBooksByIsbnsAsync(isbns, cancellationToken);
        
        return response.Data
            .Select(bookDto => bookDto.ToBook())
            .ToList();
    }
}