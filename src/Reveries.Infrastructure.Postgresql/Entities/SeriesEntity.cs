namespace Reveries.Infrastructure.Postgresql.Entities;

public class SeriesEntity
{
    public int SeriesId { get; set; }
    public string? SeriesName { get; set; }
    public DateTimeOffset? DateCreatedSeries { get; set; }
}