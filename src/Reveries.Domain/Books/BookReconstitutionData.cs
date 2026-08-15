
namespace Reveries.Domain;

public sealed record BookReconstitutionData(
    Guid Id,
    string Title,
    string? Isbn13,
    string? Isbn10,
    int? Pages,
    string? PublicationDate,
    string? Language,
    string? Synopsis,
    string? ImageThumbnailUrl,
    string? CoverImageUrl,
    decimal? Msrp,
    string? Binding,
    string? Edition,
    int? SeriesNumber,
    BookDimensions? Dimensions,
    DataSource DataSource,
    
    Publisher? Publisher = null,
    Series? Series = null,
    IEnumerable<Author>? Authors = null,
    IEnumerable<Genre>? Genres = null,
    IEnumerable<DeweyDecimal>? DeweyDecimals = null,
    DateTimeOffset? DateCreated = null
);
