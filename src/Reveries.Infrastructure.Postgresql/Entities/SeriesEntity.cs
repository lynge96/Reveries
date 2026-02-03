namespace Reveries.Infrastructure.Postgresql.Entities;

public sealed class SeriesEntity
{
    public int SeriesId { get; set; }
    public Guid SeriesDomainId { get; set; }
    public string SeriesName { get; set; } = null!;
    public DateTime DateCreatedSeries { get; set; }
}