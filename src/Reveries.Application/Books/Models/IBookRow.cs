namespace Reveries.Application.Books.Models;

/// <summary>
/// The flat set of fields a book presentation surface reads, shared by the persisted
/// read model (<see cref="BookDetails"/>) and the un-persisted enrichment carrier
/// (<see cref="BookCandidate"/>) so a single display can render either.
/// </summary>
public interface IBookRow
{
    string? Isbn13 { get; }
    string? Isbn10 { get; }
    string Title { get; }
    IReadOnlyList<string> AuthorNames { get; }
    int? Pages { get; }
    string? PublicationDate { get; }
    string? PublisherName { get; }
    string? SeriesName { get; }
    int? SeriesNumber { get; }
    string FormatLabel { get; }
    string? LanguageLabel { get; }
}