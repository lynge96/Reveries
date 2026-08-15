using Reveries.Domain.Authors;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Shared;

namespace Reveries.Domain.Works;

public sealed record WorkReconstitutionData(
    Guid Id,
    string Title,
    string? Synopsis,
    int? SeriesNumber,
    Series? Series = null,
    IEnumerable<Author>? Authors = null,
    IEnumerable<Genre>? Genres = null,
    IEnumerable<DeweyDecimal>? DeweyDecimals = null,
    DateTimeOffset? DateCreated = null
);
