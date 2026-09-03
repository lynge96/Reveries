using Reveries.Domain.Authors;
using Reveries.Domain.BookSeries;

namespace Reveries.Domain.Works;

public sealed record WorkReconstitutionData(
    Guid Id,
    string Title,
    string? Subtitle,
    string? Synopsis,
    string? Description,
    int? SeriesNumber,
    SeriesId? SeriesId = null,
    IEnumerable<AuthorId>? AuthorIds = null,
    IEnumerable<Genre>? PrimaryGenres = null,
    IEnumerable<Genre>? SecondaryGenres = null,
    IEnumerable<DeweyDecimal>? DeweyDecimals = null
);
