
namespace Reveries.Domain;

public sealed record EditionReconstitutionData(
    Guid Id,
    Guid WorkId,
    string? Isbn13,
    string? Isbn10,
    int? Pages,
    string? PublicationDate,
    string? Language,
    string? EditionStatement,
    string? Binding,
    string? ImageThumbnailUrl,
    string? CoverImageUrl,
    decimal? Msrp,
    BookDimensions? Dimensions,
    DataSource DataSource,
    Publisher? Publisher = null,
    DateTimeOffset? DateCreated = null
);
