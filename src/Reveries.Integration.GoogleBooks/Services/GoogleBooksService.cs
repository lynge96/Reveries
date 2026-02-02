using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.GoogleBooks;
using Reveries.Core.Enums;
using Reveries.Core.Exceptions;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Integration.GoogleBooks.DTOs;
using Reveries.Integration.GoogleBooks.Interfaces;
using Reveries.Integration.GoogleBooks.Mappers;

namespace Reveries.Integration.GoogleBooks.Services;

public class GoogleBooksService : IGoogleBooksService
{
    private readonly IGoogleBooksClient _googleBooksClient;
    private readonly ILogger<GoogleBooksService> _logger;
    
    public GoogleBooksService(IGoogleBooksClient googleBooksClient, ILogger<GoogleBooksService> logger)
    {
        _googleBooksClient = googleBooksClient;
        _logger = logger;
    }
    
    public async Task<List<Book>> GetBooksByIsbnsAsync(List<string> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return [];
        
        var tasks = isbns.Select(async isbn =>
        {
            try
            {
                var response = await _googleBooksClient.FetchBookByIsbnAsync(isbn, ct);

                if (response.Items == null || response.Items.Count == 0)
                {
                    _logger.LogDebug("ISBN '{Isbn}' returned 0 items.", isbn);
                    return null;
                }

                var item = response.Items.First();

                GoogleBookItemDto volumeResponse;
                try
                {
                    volumeResponse = await _googleBooksClient.FetchBookByVolumeIdAsync(item.Id, ct);
                }
                catch (NotFoundException)
                {
                    _logger.LogDebug("Volume '{VolumeId}' for ISBN '{Isbn}' not found. Using primary volume info only.", item.Id, isbn);
                    return item.VolumeInfo.ToBook();
                }
                
                var primaryBook = item.VolumeInfo.ToBook();
                var volumeBook = volumeResponse.VolumeInfo.ToBook();

                return MergeGoogleBooks(primaryBook, volumeBook);
            }
            catch (NotFoundException)
            {
                _logger.LogDebug("ISBN '{Isbn}' not found.", isbn);
                return null;
            }
        });
        
        var results = await Task.WhenAll(tasks);
        var books = results.Where(b => b != null).Select(b => b!).ToList();

        _logger.LogDebug("Completed GoogleBooks ISBN lookup. Requested {RequestedCount} ISBNs, found {FoundCount} books.", isbns.Count, books.Count);
        return books;
    }

    public async Task<List<Book>> GetBooksByTitleAsync(List<string> titles, CancellationToken ct)
    {
        if (titles.Count == 0)
            return [];

        var tasks = titles.Select(async title =>
        {
            try
            {
                var response = await _googleBooksClient.SearchBooksByTitleAsync(title, ct);

                if (response.Items == null || response.Items.Count == 0)
                    return [];

                return response.Items.Select(i => i.VolumeInfo.ToBook());
            }
            catch (NotFoundException)
            {
                _logger.LogDebug("GoogleBooks returned no results for title '{Title}'.", title);
                return [];
            }
        });

        var results = await Task.WhenAll(tasks);

        var flattened = results.SelectMany(x => x).ToList();
        
        _logger.LogDebug(
            "GoogleBooks title lookup completed. Searched {TotalTitles} titles, found {TotalBooks} books. Titles: {Titles}",
            titles.Count,
            flattened.Count,
            string.Join(", ", titles)
        );

        return flattened;
    }

    private static Book MergeGoogleBooks(Book book, Book volume)
    {
        var mergedTitle = !string.IsNullOrWhiteSpace(book.Title)
            ? book.Title
            : volume.Title;
    
        var mergedAuthors = book.Authors.Count > 0
            ? book.Authors
            : volume.Authors;
    
        var mergedSubjects = (volume.Genres?.Count ?? 0) > 0
            ? volume.Genres
            : book.Genres;
    
        var mergedDeweyDecimals = (volume.DeweyDecimals?.Count ?? 0) > 0
            ? volume.DeweyDecimals
            : book.DeweyDecimals;
    
        var mergedSynopsis = (volume.Synopsis?.Length ?? 0) > (book.Synopsis?.Length ?? 0)
            ? volume.Synopsis
            : book.Synopsis;
    
        var mergedPages = book.Pages > 0
            ? book.Pages
            : volume.Pages > 0 ? volume.Pages : null;
    
        var dimensions = volume.Dimensions ?? book.Dimensions;

        return Book.Reconstitute(
            id: book.Id,
            isbn13: book.Isbn13 ?? volume.Isbn13,
            isbn10: book.Isbn10 ?? volume.Isbn10,
            title: mergedTitle,
            pages: mergedPages,
            isRead: false,
            publishDate: book.PublishDate ?? volume.PublishDate,
            language: book.Language ?? volume.Language,
            synopsis: mergedSynopsis,
            imageThumbnail: book.ImageThumbnailUrl ?? volume.ImageThumbnailUrl,
            imageUrl: book.CoverImageUrl ?? volume.CoverImageUrl,
            msrp: book.Msrp ?? volume.Msrp,
            binding: book.Binding,
            edition: book.Edition,
            seriesNumber: book.SeriesNumber,
            dataSource: DataSource.GoogleBooksApi,
            publisher: book.Publisher ?? volume.Publisher,
            series: book.Series,
            dimensions: BookDimensions.Create(
                dimensions?.HeightCm,
                dimensions?.WidthCm,
                dimensions?.ThicknessCm,
                dimensions?.WeightG
            ),
            authors: mergedAuthors,
            subjects: mergedSubjects,
            deweyDecimals: mergedDeweyDecimals!
        );
    }
}