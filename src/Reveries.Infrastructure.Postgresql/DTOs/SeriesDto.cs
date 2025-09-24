namespace Reveries.Infrastructure.Postgresql.DTOs;

public class SeriesDto
{
    public int SeriesId { get; set; }
    public string SeriesName { get; set; } = null!;
    public DateTimeOffset DateCreatedSeries { get; set; }
}