using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reveries.Application.Interfaces.Isbndb;
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
    private readonly ILogger<IsbndbBookService> _logger;

    public IsbndbBookService(IIsbndbBookClient bookClient, IOptions<IsbndbSettings> options, ILogger<IsbndbBookService> logger)
    {
        _bookClient = bookClient;
        _settings = options.Value;
        _logger = logger;
    }
    
    public async Task<List<Book>> GetBooksByIsbnsAsync(List<string> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return [];

        if (isbns.Count > _settings.MaxBulkIsbns)
        {
            throw new IsbnValidationException($"Too many ISBN numbers. Maximum is {_settings.MaxBulkIsbns}.");
        }
        
        if (isbns.Count == 1)
        {
            var book = await GetSingleBookAsync(isbns[0], ct);
            _logger.LogDebug("{Service} Single ISBN lookup for '{Isbn}' succeeded.", nameof(GetBooksByIsbnsAsync), isbns[0]);

            return [book];
        }
        
        var books = await GetMultipleBooksAsync(isbns, ct);

        _logger.LogDebug("Bulk ISBN lookup requested {Requested} ISBNs and returned {Found} books.", isbns.Count, books.Count);

        return books;
    }
    
    public async Task<List<Book>> GetBooksByTitlesAsync(List<string> titles, string? languageCode, CancellationToken ct)
    {
        if (titles.Count == 0)
            return [];
        
        var tasks = titles.Select(async title =>
        {
            try
            {
                var response = await _bookClient.SearchBooksAsync(title, languageCode, shouldMatchAll: true, ct);

                var bookDtos = response.Books;

                var mapped = bookDtos
                    .Select(b => b.ToBook())
                    .ToList();

                _logger.LogDebug("Title search '{Title}' returned {Count} books.", title, mapped.Count);

                return mapped;
            }
            catch (NotFoundException)
            {
                _logger.LogDebug("Title search '{Title}' returned no results.", title);

                return [];
            }
        });

        var results = await Task.WhenAll(tasks);

        var allBooks = results
            .SelectMany(b => b)
            .ToList();

        _logger.LogDebug("Completed title search. Requested {RequestedTitles} titles, found {TotalBooks} books.", titles.Count, allBooks.Count);

        return allBooks;
    }

    private async Task<Book> GetSingleBookAsync(string isbn, CancellationToken ct)
    {
        var dto = await _bookClient.FetchBookByIsbnAsync(isbn, ct);

        var book = dto.Book.ToBook();

        return book;
    }
    
    private async Task<List<Book>> GetMultipleBooksAsync(List<string> isbns, CancellationToken ct)
    {
        var response = await _bookClient.FetchBooksByIsbnsAsync(isbns, ct);
        
        var books = response.Data
            .Select(b => b.ToBook())
            .ToList();

        return books;
    }
}