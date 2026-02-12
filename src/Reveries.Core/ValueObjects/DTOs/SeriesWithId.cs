using Reveries.Core.Models;

namespace Reveries.Core.ValueObjects.DTOs;

public record SeriesWithId
{
    public Series Series { get; init; }
    public int DbId { get; init; }

    public SeriesWithId(Series series, int dbId)
    {
        Series = series;
        DbId = dbId;
    }
}