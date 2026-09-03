using Reveries.Domain.BookSeries;
using Reveries.Domain.Exceptions;

namespace Reveries.Domain.Works;

public sealed record SeriesPlacement
{
    public Series Series { get; }
    public int? Number { get; }

    internal SeriesPlacement(Series series, int? number)
    {
        Series = series;
        Number = number;
    }

    public static SeriesPlacement Create(Series series, int? number = null)
    {
        ArgumentNullException.ThrowIfNull(series);

        if (number is <= 0)
            throw new InvalidSeriesNumberException(number);

        return new SeriesPlacement(series, number);
    }
}