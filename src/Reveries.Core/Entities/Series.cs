namespace Reveries.Core.Entities;

public class Series
{
    public int SeriesId { get; set; }

    public string Name { get; set; } = null!;
    
    public DateTimeOffset DateCreatedSeries { get; set; }
}