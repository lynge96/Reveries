namespace Reveries.Persistence.Entities;

public class WorkAggregateEntity
{
    public WorkEntity Work { get; set; } = null!;
    public SeriesEntity? Series { get; set; }
    public List<AuthorEntity>? Authors { get; set; } = new();
    public List<GenreEntity>? Genres { get; set; } = new();
    public List<DeweyDecimalEntity>? DeweyDecimals { get; set; } = new();
}