using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reveries.Application.Books.Interfaces;
using Reveries.Core.Exceptions;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Integration.Isbndb.Configuration;
using Reveries.Integration.Isbndb.Interfaces;
using Reveries.Integration.Isbndb.Mappers;

namespace Reveries.Integration.Isbndb.Services;

public class IsbndbBookService : IIsbndbBookSearch
{
    private readonly IIsbndbBookClient _bookClient;
    private readonly IsbndbSettings _settings;
    private readonly ILogger<IsbndbBookService> _logger;

    public IsbndbBookService(IIsbndbBookClient bookClient, IOptions<IsbndbSettings> options, ILogger<IsbndbBookService> logger)
    {
        _bookClient = bookClient;
        _settings = options.Value;
        _logger = logger;
    }
    
    public async Task<List<Book>?> GetBooksByIsbnsAsync(List<Isbn> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return [];

        if (isbns.Count > _settings.MaxBulkIsbns)
            throw new InvalidIsbnException($"Too many ISBN numbers. Maximum is {_settings.MaxBulkIsbns}.");
        
        if (isbns.Count == 1)
        {
            var isbn = isbns.First();
            
            var book = await GetSingleBookAsync(isbn, ct);
            
            if (book is null)
                return null;
            
            _logger.LogDebug("Single ISBN lookup for '{Isbn}' succeeded.", isbn);
            return [book];
        }
        
        var books = await GetMultipleBooksAsync(isbns, ct);

        if (books is null)
            return null;
        
        _logger.LogDebug("Bulk ISBN lookup requested {Requested} ISBNs and returned {Found} books.", isbns.Count, books.Count);
        return books;
    }
    
    public async Task<List<Book>?> GetBooksByTitlesAsync(List<string> titles, string? languageCode, CancellationToken ct)
    {
        if (titles.Count == 0)
            return [];
        
        var tasks = titles.Select(async title =>
        {
            var response = await _bookClient.SearchBooksAsync(title, languageCode, shouldMatchAll: true, ct: ct);

            var mapped = response?.Books
                .Select(b => b.ToBook())
                .ToList();
            
            return mapped;
        });

        var results = await Task.WhenAll(tasks);
        
        if (results.All(r => r is null))
            return null;
        
        var allBooks = results
            .Where(r => r is not null)
            .SelectMany(b => b!)
            .ToList();

        _logger.LogDebug("Completed title search. Requested {RequestedTitles} titles, found {TotalBooks} books.", titles.Count, allBooks.Count);
        return allBooks;
    }

    private async Task<Book?> GetSingleBookAsync(Isbn isbn, CancellationToken ct)
    {
        var dto = await _bookClient.FetchBookByIsbnAsync(isbn, ct);

        var book = dto?.Book.ToBook();

        return book;
    }
    
    private async Task<List<Book>?> GetMultipleBooksAsync(List<Isbn> isbns, CancellationToken ct)
    {
        var response = await _bookClient.FetchBooksByIsbnsAsync(isbns, ct);
        
        var books = response?.Data
            .Select(b => b.ToBook())
            .ToList();

        return books;
    }

}