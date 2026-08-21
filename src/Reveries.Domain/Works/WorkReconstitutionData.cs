using Reveries.Domain.Authors;
using Reveries.Domain.BookSeries;

namespace Reveries.Domain.Works;

public sealed record WorkReconstitutionData(
    Guid Id,
    string Title,
    string? Synopsis,
    int? SeriesNumber,
    Series? Series = null,
    IEnumerable<Author>? Authors = null,
    IEnumerable<Genre>? PrimaryGenres = null,
    IEnumerable<Genre>? SecondaryGenres = null,
    IEnumerable<DeweyDecimal>? DeweyDecimals = null
);
