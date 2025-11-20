using Microsoft.Extensions.Logging;
using Reveries.Application.Interfaces.GoogleBooks;
using Reveries.Core.Enums;
using Reveries.Core.Exceptions;
using Reveries.Core.Models;
using Reveries.Integration.GoogleBooks.Interfaces;
using Reveries.Integration.GoogleBooks.Mappers;

namespace Reveries.Integration.GoogleBooks.Services;

public class GoogleBookService : IGoogleBookService
{
    private readonly IGoogleBooksClient _googleBooksClient;
    private readonly ILogger<GoogleBookService> _logger;
    
    public GoogleBookService(IGoogleBooksClient googleBooksClient, ILogger<GoogleBookService> logger)
    {
        _googleBooksClient = googleBooksClient;
        _logger = logger;
    }
    
    public async Task<List<Book>> GetBooksByIsbnsAsync(List<string> isbns, CancellationToken ct)
    {
        var tasks = isbns.Select(async isbn =>
        {
            var response = await _googleBooksClient.FetchBookByIsbnAsync(isbn, ct);
            if (response?.Items == null || response.Items.Count == 0)
                return null;

            var item = response.Items.First();

            var volumeResponse = await _googleBooksClient.FetchBookByVolumeIdAsync(item.Id, ct);
            if (volumeResponse?.VolumeInfo == null)
                return item.VolumeInfo.ToBook();

            var primaryBook = item.VolumeInfo.ToBook();
            var volumeBook = volumeResponse.VolumeInfo.ToBook();

            return MergeGoogleBooks(primaryBook, volumeBook);
        });
        
        var books = await Task.WhenAll(tasks);
        
        return books.Where(b => b != null).ToList()!;
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
        return new Book
        {
            DataSource = DataSource.GoogleBooksApi,
            Title = book.Title,
            Isbn13 = book.Isbn13 ?? volume.Isbn13,
            Isbn10 = book.Isbn10 ?? volume.Isbn10,
            Pages =  (book.Pages > 0) ? book.Pages : volume.Pages,
            Synopsis = (volume.Synopsis?.Length ?? 0) > (book.Synopsis?.Length ?? 0)
                ? volume.Synopsis
                : book.Synopsis,
            Authors = book.Authors,
            Edition = book.Edition,
            Publisher = book.Publisher ?? volume.Publisher,
            PublishDate = book.PublishDate ?? volume.PublishDate,
            Subjects = volume.Subjects,
            Language = book.Language ?? volume.Language,
            Binding = book.Binding,
            ImageThumbnail = book.ImageThumbnail ?? volume.ImageThumbnail,
            Dimensions = volume.Dimensions
        };
    }
}