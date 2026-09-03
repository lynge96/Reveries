using Reveries.Domain.Authors;
using Reveries.Domain.Editions;
using Reveries.Domain.Enums;
using Reveries.Domain.Helpers;
using Reveries.Domain.Works;
using DomainDescription = Reveries.Domain.Works.Description;
using DomainPublicationDate = Reveries.Domain.Editions.PublicationDate;
using DomainPublisher = Reveries.Domain.Publishers.Publisher;
using DomainSynopsis = Reveries.Domain.Works.Synopsis;
using DomainTitle = Reveries.Domain.Works.Title;

namespace Reveries.Application.Books.Models;

/// <summary>
/// Flat, identity-free representation of book metadata gathered from an external source
/// (ISBNDB, Google Books) before it is persisted. Aggregate references (authors, publisher)
/// are carried as normalized names; genuine value objects are carried as-is.
/// </summary>
public sealed record BookCandidate
{
    public Isbn? Isbn { get; init; }
    public required string Title { get; init; }
    public string? Subtitle { get; init; }
    public IReadOnlyList<string> Authors { get; init; } = [];
    public string? Publisher { get; init; }
    public IReadOnlyList<string> PrimaryGenres { get; init; } = [];
    public IReadOnlyList<string> SecondaryGenres { get; init; } = [];
    public IReadOnlyList<string> DeweyDecimals { get; init; } = [];
    public string? Synopsis { get; init; }
    public string? Description { get; init; }
    public int? Pages { get; init; }
    public string? PublicationDate { get; init; }
    public Language? Language { get; init; }
    public BookFormat Format { get; init; }
    public string? EditionStatement { get; init; }
    public Cover? Cover { get; init; }
    public BookDimensions? Dimensions { get; init; }

    public static BookCandidate Create(BookCandidateData data)
    {
        var genres = GenreClassification.Create(data.PrimaryGenres, data.SecondaryGenres);

        return new BookCandidate
        {
            Isbn = ResolveIsbn(data.Isbn13, data.Isbn10),
            Title = DomainTitle.Create(data.Title).Text,
            Subtitle = string.IsNullOrWhiteSpace(data.Subtitle) ? null : data.Subtitle.Trim(),
            Authors = NormalizeAuthors(data.Authors),
            Publisher = DomainPublisher.TryCreate(data.Publisher)?.Name,
            PrimaryGenres = genres.Primary.Select(g => g.Name).ToList(),
            SecondaryGenres = genres.Secondary.Select(g => g.Name).ToList(),
            DeweyDecimals = NormalizeDeweyDecimals(data.DeweyDecimals),
            Synopsis = DomainSynopsis.TryCreate(data.Synopsis)?.Text,
            Description = DomainDescription.TryCreate(data.Description)?.Text,
            Pages = PageCountNormalizer.Normalize(data.Pages),
            PublicationDate = DomainPublicationDate.TryCreate(data.PublishDate)?.Value,
            Language = Language.TryCreate(data.LanguageIso639),
            Format = data.Format.GetStandardFormat(),
            EditionStatement = EditionDescriptionNormalizer.Normalize(data.EditionStatement),
            Cover = Cover.TryCreate(url: data.ImageUrl, thumbnailUrl: data.ImageThumbnail),
            Dimensions = data.Dimensions
        };
    }

    private static Isbn? ResolveIsbn(string? isbn13, string? isbn10)
    {
        if (!string.IsNullOrWhiteSpace(isbn13))
            return Isbn.Create(isbn13);

        return string.IsNullOrWhiteSpace(isbn10) ? null : Isbn.Create(isbn10);
    }

    private static IReadOnlyList<string> NormalizeAuthors(IEnumerable<string>? names)
    {
        return (names ?? [])
            .Select(Author.TryCreate)
            .OfType<Author>()
            .GroupBy(author => author.NormalizedName)
            .Select(group => group.First().Name)
            .ToList();
    }

    private static IReadOnlyList<string> NormalizeDeweyDecimals(IEnumerable<string>? codes)
    {
        return (codes ?? [])
            .Select(DeweyDecimal.TryCreate)
            .OfType<DeweyDecimal>()
            .GroupBy(dewey => dewey.Code)
            .Select(group => group.First().Code)
            .ToList();
    }
}