namespace Reveries.Infrastructure.Persistence.DTOs;

public class SeriesDto
{
    public int SeriesId { get; set; }
    public string SeriesName { get; set; } = null!;
    public DateTime DateCreatedSeries { get; set; }
}