using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Enums;
using Reveries.Domain.Works;
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

    public async Task<List<EditionWithWork>?> GetBooksByIsbnsAsync(IReadOnlyList<Isbn> isbns, CancellationToken ct)
    {
        if (isbns.Count == 0)
            return [];

        var tasks = isbns.Select(isbn => FetchAndMergeByIsbnAsync(isbn, ct));
        var results = await Task.WhenAll(tasks);

        if (results.All(r => r is null))
            return null;

        var books = results
            .OfType<EditionWithWork>()
            .ToList();

        _logger.LogDebug("GoogleBooks ISBN lookup completed. Requested {RequestedCount} ISBNs, found {FoundCount} books.", isbns.Count, books.Count);
        return books;
    }

    public async Task<List<EditionWithWork>?> GetBooksByTitlesAsync(IReadOnlyList<Title> titles, CancellationToken ct)
    {
        if (titles.Count == 0)
            return [];

        var tasks = titles.Select(title => FetchAndMergeByTitleAsync(title, ct));
        var results = await Task.WhenAll(tasks);

        if (results.All(r => r is null))
            return null;

        var books = results
            .OfType<EditionWithWork>()
            .ToList();

        _logger.LogDebug("GoogleBooks title lookup completed. Searched {TotalTitles} titles, found {TotalBooks} books.", titles.Count, books.Count);

        return books;
    }

    private async Task<EditionWithWork?> FetchAndMergeByIsbnAsync(Isbn isbn, CancellationToken ct)
    {
        var bookResponse = await _googleBooksClient.FetchBookByIsbnAsync(isbn, ct);

        if (bookResponse?.Items is null)
        {
            _logger.LogDebug("ISBN '{Isbn}' not found in Google Books.", isbn);
            return null;
        }

        return await FetchVolumeAndMergeAsync(bookResponse.Items.First(), ct);
    }

    private async Task<EditionWithWork?> FetchAndMergeByTitleAsync(Title title, CancellationToken ct)
    {
        var bookResponse = await _googleBooksClient.SearchBooksByTitleAsync(title, ct);

        if (bookResponse?.Items is null)
        {
            _logger.LogDebug("GoogleBooks returned no results for title '{Title}'.", title);
            return null;
        }

        return await FetchVolumeAndMergeAsync(bookResponse.Items.First(), ct);
    }

    private async Task<EditionWithWork?> FetchVolumeAndMergeAsync(GoogleBookItemDto item, CancellationToken ct)
    {
        var volumeResponse = await _googleBooksClient.FetchBookByVolumeIdAsync(item.Id, ct);

        var primary = item.VolumeInfo.ToEditionWithWork();
        var volume = volumeResponse?.VolumeInfo.ToEditionWithWork();

        return MergeGoogleEditions(primary, volume);
    }

    private static EditionWithWork? MergeGoogleEditions(EditionWithWork? primary, EditionWithWork? volume)
    {
        if (primary is null && volume is null)
            return null;
        if (primary is null)
            return volume;
        if (volume is null)
            return primary;

        var pw = primary.Work;
        var vw = volume.Work;
        var pe = primary.Edition;
        var ve = volume.Edition;

        var work = Work.Reconstitute(new WorkReconstitutionData(
            Id: pw.Id.Value,
            Title: Prefer(pw.Title.Value, vw.Title.Value) ?? string.Empty,
            Synopsis: Longest(vw.Synopsis?.Value, pw.Synopsis?.Value),
            SeriesNumber: pw.SeriesPlacement?.Number,
            Series: pw.SeriesPlacement?.Series,
            Authors: pw.Authors.Count != 0 ? pw.Authors : vw.Authors,
            Genres: vw.Genres.Count != 0 ? vw.Genres : pw.Genres,
            DeweyDecimals: vw.DeweyDecimals.Count != 0 ? vw.DeweyDecimals : pw.DeweyDecimals));

        var edition = Edition.Reconstitute(new EditionReconstitutionData(
            Id: pe.Id.Value,
            WorkId: work.Id.Value,
            Isbn13: pe.Isbn13?.Value ?? ve.Isbn13?.Value,
            Isbn10: pe.Isbn10?.Value ?? ve.Isbn10?.Value,
            Pages: pe.Pages > 0 ? pe.Pages : ve.Pages,
            PublicationDate: pe.PublicationDate ?? ve.PublicationDate,
            Language: pe.Language ?? ve.Language,
            EditionStatement: pe.EditionStatement ?? ve.EditionStatement,
            Binding: pe.Binding ?? ve.Binding,
            ImageThumbnailUrl: pe.ImageThumbnailUrl ?? ve.ImageThumbnailUrl,
            CoverImageUrl: pe.CoverImageUrl ?? ve.CoverImageUrl,
            Msrp: pe.Msrp ?? ve.Msrp,
            Dimensions: ve.Dimensions ?? pe.Dimensions,
            DataSource: DataSource.GoogleBooksApi,
            Publisher: pe.Publisher ?? ve.Publisher));

        return new EditionWithWork(edition, work);
    }

    private static string? Prefer(params string?[] values)
        => values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

    private static string? Longest(string? a, string? b)
        => (a?.Length ?? 0) >= (b?.Length ?? 0) ? a : b;
}
