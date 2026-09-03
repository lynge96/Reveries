namespace Reveries.Persistence.Entities;

public sealed class WorkEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Subtitle { get; set; }
    public string? Synopsis { get; set; }
    public string? Description { get; set; }
    public int? SeriesNumber { get; set; }
    public Guid? SeriesId { get; set; }
    public DateTimeOffset? DateCreated { get; set; }
}