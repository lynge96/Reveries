using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Models;

public sealed record BookCandidateData(
    string? Isbn13,
    string? Isbn10,
    string Title,
    string? Subtitle,
    IEnumerable<string>? Authors,
    string? Publisher,
    IEnumerable<string>? PrimaryGenres,
    IEnumerable<string>? SecondaryGenres,
    IEnumerable<string>? DeweyDecimals,
    string? Synopsis,
    string? Description,
    int? Pages,
    string? PublishDate,
    string? LanguageIso639,
    string? Format,
    string? EditionStatement,
    string? ImageThumbnail,
    string? ImageUrl,
    BookDimensions? Dimensions);