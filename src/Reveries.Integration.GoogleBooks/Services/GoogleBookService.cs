using Microsoft.Extensions.Logging;
using Reveries.Application.Interfaces.Books;
using Reveries.Core.Enums;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Integration.GoogleBooks.DTOs;
using Reveries.Integration.GoogleBooks.Interfaces;
using Reveries.Integration.GoogleBooks.Mappers;

namespace Reveries.Integration.GoogleBooks.Services;

public class GoogleBookService : IGoogleBookSearch
{
    private readonly IGoogleBooksClient _googleBooksClient;
    private readonly ILogger<GoogleBookService> _logger;
    
    public GoogleBookService(IGoogleBooksClient googleBooksClient, ILogger<GoogleBookService> logger)
    {
        _googleBooksClient = googleBooksClient;
        _logger = logger;
    }
    
    public async Task<List<Book>?> GetBooksByIsbnsAsync(List<Isbn> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return [];

        var tasks = isbns.Select(isbn => FetchAndMergeByIsbnAsync(isbn, ct));
        var results = await Task.WhenAll(tasks);

        if (results.All(r => r is null))
            return null;

        var books = results
            .Where(b => b is not null)
            .Select(b => b!)
            .ToList();

        _logger.LogDebug("GoogleBooks ISBN lookup completed. Requested {RequestedCount} ISBNs, found {FoundCount} books.", isbns.Count, books.Count);
        return books;
    }

    public async Task<List<Book>?> GetBooksByTitleAsync(List<string> titles, CancellationToken ct)
    {
        if (titles.Count == 0)
            return [];

        var tasks = titles.Select(title => FetchAndMergeByTitleAsync(title, ct));
        var results = await Task.WhenAll(tasks);

        if (results.All(r => r is null))
            return null;

        var books = results
            .Where(b => b is not null)
            .Select(b => b!)
            .ToList();

        _logger.LogDebug("GoogleBooks title lookup completed. Searched {TotalTitles} titles, found {TotalBooks} books.", titles.Count, books.Count);

        return books;
    }

    private async Task<Book?> FetchAndMergeByIsbnAsync(Isbn isbn, CancellationToken ct)
    {
        var bookResponse = await _googleBooksClient.FetchBookByIsbnAsync(isbn, ct);

        if (bookResponse?.Items is null)
        {
            _logger.LogDebug("ISBN '{Isbn}' not found in Google Books.", isbn);
            return null;
        }

        return await FetchVolumeAndMergeAsync(bookResponse.Items.First(), ct);
    }

    private async Task<Book?> FetchAndMergeByTitleAsync(string title, CancellationToken ct)
    {
        var bookResponse = await _googleBooksClient.SearchBooksByTitleAsync(title, ct);

        if (bookResponse?.Items is null)
        {
            _logger.LogDebug("GoogleBooks returned no results for title '{Title}'.", title);
            return null;
        }

        return await FetchVolumeAndMergeAsync(bookResponse.Items.First(), ct);
    }

    private async Task<Book?> FetchVolumeAndMergeAsync(GoogleBookItemDto item, CancellationToken ct)
    {
        var volumeResponse = await _googleBooksClient.FetchBookByVolumeIdAsync(item.Id, ct);

        var primaryBook = item.VolumeInfo.ToBook();
        var volumeBook = volumeResponse?.VolumeInfo.ToBook();

        return MergeGoogleBooks(primaryBook, volumeBook);
    }

    private static Book MergeGoogleBooks(Book book, Book? volume)
    {
        var mergedTitle = !string.IsNullOrWhiteSpace(book.Title)
            ? book.Title
            : volume?.Title ?? throw new InvalidOperationException($"Book title is missing from both sources, for book with ISBN '{book.Isbn13?.Value ?? volume?.Isbn13?.Value}'.");
    
        var mergedAuthors = book.Authors.Count > 0
            ? book.Authors
            : volume?.Authors;
    
        var mergedSubjects = volume?.Genres.Count != 0
            ? volume?.Genres
            : book.Genres;
    
        var mergedDeweyDecimals = volume?.DeweyDecimals.Count != 0
            ? volume?.DeweyDecimals
            : book.DeweyDecimals;
    
        var mergedSynopsis = (volume?.Synopsis?.Length ?? 0) > (book.Synopsis?.Length ?? 0)
            ? volume?.Synopsis
            : book.Synopsis;
    
        var mergedPages = book.Pages > 0
            ? book.Pages
            : volume?.Pages > 0 ? volume.Pages : null;
    
        var dimensions = volume?.Dimensions ?? book.Dimensions;

        var bookData = new BookReconstitutionData
        (
            Id: book.Id.Value,
            Isbn13: book.Isbn13?.Value ?? volume?.Isbn13?.Value,
            Isbn10: book.Isbn10?.Value ?? volume?.Isbn10?.Value,
            Title: mergedTitle,
            Pages: mergedPages,
            IsRead: false,
            PublicationDate: book.PublicationDate ?? volume?.PublicationDate,
            Language: book.Language ?? volume?.Language,
            Synopsis: mergedSynopsis,
            ImageThumbnailUrl: book.ImageThumbnailUrl ?? volume?.ImageThumbnailUrl,
            CoverImageUrl: book.CoverImageUrl ?? volume?.CoverImageUrl,
            Msrp: book.Msrp ?? volume?.Msrp,
            Binding: book.Binding,
            Edition: book.Edition,
            SeriesNumber: book.SeriesNumber,
            DataSource: DataSource.GoogleBooksApi,
            Publisher: book.Publisher ?? volume?.Publisher,
            Series: book.Series,
            Dimensions: BookDimensions.Create(
                dimensions?.HeightCm,
                dimensions?.WidthCm,
                dimensions?.ThicknessCm,
                dimensions?.WeightG
            ),
            Authors: mergedAuthors,
            Genres: mergedSubjects,
            DeweyDecimals: mergedDeweyDecimals
        );
        
        return Book.Reconstitute(bookData);
    }
}