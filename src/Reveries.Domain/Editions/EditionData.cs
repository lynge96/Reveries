using Reveries.Domain.Enums;
using Reveries.Domain.Works;

namespace Reveries.Domain.Editions;

public sealed record EditionData(
    WorkId WorkId,
    string? Isbn13,
    string? Isbn10,
    string? Publisher,
    int? Pages,
    string? PublishDate,
    string? LanguageIso639,
    string? Format,
    string? EditionStatement,
    string? ImageThumbnail,
    string? ImageUrl,
    string? SaxoUrl,
    decimal? Msrp,
    BookDimensions? Dimensions,
    DataSource DataSource);