using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Enums;
using Reveries.Integration.GoogleBooks.DTOs;
using Reveries.Integration.GoogleBooks.Interfaces;
using Reveries.Integration.GoogleBooks.Mappers;

namespace Reveries.Integration.GoogleBooks.Services;

public class GoogleBookService : IBookSearch
{
    public BookSource Source => BookSource.GoogleBooks;

    private readonly IGoogleBooksClient _googleBooksClient;
    private readonly ILogger<GoogleBookService> _logger;

    public GoogleBookService(IGoogleBooksClient googleBooksClient, ILogger<GoogleBookService> logger)
    {
        _googleBooksClient = googleBooksClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<BookCandidate>?> GetBooksByIsbnsAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return [];

        var tasks = isbns.Select(isbn => FetchAndMergeByIsbnAsync(isbn, ct));
        var results = await Task.WhenAll(tasks);

        if (results.All(r => r is null))
            return null;

        return results
            .OfType<BookCandidate>()
            .ToList();
    }

    private async Task<BookCandidate?> FetchAndMergeByIsbnAsync(Isbn isbn, CancellationToken ct)
    {
        var bookResponse = await _googleBooksClient.FetchBookByIsbnAsync(isbn, ct);

        var item = bookResponse?.Items?.FirstOrDefault();
        if (item is null)
        {
            _logger.LogDebug("ISBN '{Isbn}' not found in Google Books.", isbn);
            return null;
        }

        return await FetchVolumeAndMergeAsync(item, ct);
    }

    private async Task<BookCandidate?> FetchVolumeAndMergeAsync(GoogleBookItemDto item, CancellationToken ct)
    {
        var primary = item.VolumeInfo?.ToBookCandidate();

        if (string.IsNullOrWhiteSpace(item.Id))
            return primary;

        var volumeResponse = await _googleBooksClient.FetchBookByVolumeIdAsync(item.Id, ct);
        var volume = volumeResponse?.VolumeInfo?.ToBookCandidate();

        return MergeGoogleCandidates(primary, volume);
    }

    private static BookCandidate? MergeGoogleCandidates(BookCandidate? primary, BookCandidate? volume)
    {
        if (primary is null && volume is null)
            return null;
        if (primary is null)
            return volume;
        if (volume is null)
            return primary;

        return new BookCandidate
        {
            Isbn = primary.Isbn ?? volume.Isbn,
            Title = Prefer(primary.Title, volume.Title) ?? string.Empty,
            Subtitle = Prefer(primary.Subtitle, volume.Subtitle),
            Authors = primary.Authors.Count != 0 ? primary.Authors : volume.Authors,
            Publisher = Prefer(primary.Publisher, volume.Publisher),
            PrimaryGenres = volume.PrimaryGenres.Count != 0 ? volume.PrimaryGenres : primary.PrimaryGenres,
            SecondaryGenres = volume.SecondaryGenres.Count != 0 ? volume.SecondaryGenres : primary.SecondaryGenres,
            DeweyDecimals = volume.DeweyDecimals.Count != 0 ? volume.DeweyDecimals : primary.DeweyDecimals,
            Synopsis = Prefer(primary.Synopsis, volume.Synopsis),
            Description = Prefer(volume.Description, primary.Description),
            Pages = primary.Pages > 0 ? primary.Pages : volume.Pages,
            PublicationDate = Prefer(primary.PublicationDate, volume.PublicationDate),
            Language = primary.Language ?? volume.Language,
            Format = PreferFormat(primary.Format, volume.Format),
            EditionStatement = Prefer(primary.EditionStatement, volume.EditionStatement),
            Cover = Cover.TryCreate(
                url: primary.Cover?.Url ?? volume.Cover?.Url,
                thumbnailUrl: primary.Cover?.ThumbnailUrl ?? volume.Cover?.ThumbnailUrl),
            Dimensions = volume.Dimensions ?? primary.Dimensions
        };
    }

    private static string? Prefer(params string?[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
    }

    private static BookFormat PreferFormat(BookFormat first, BookFormat second)
    {
        return first != BookFormat.Unknown ? first : second;
    }
}