using Reveries.Domain.Publishers;
using Reveries.Domain.Works;

namespace Reveries.Domain.Editions;

public sealed record EditionData(
    WorkId WorkId,
    string? Isbn13,
    string? Isbn10,
    PublisherId? PublisherId,
    int? Pages,
    string? PublishDate,
    string? LanguageIso639,
    string? Format,
    string? EditionStatement,
    string? ImageThumbnail,
    string? ImageUrl,
    string? SaxoUrl,
    BookDimensions? Dimensions);