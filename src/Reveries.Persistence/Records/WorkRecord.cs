namespace Reveries.Persistence.Records;

public sealed class WorkRecord
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Subtitle { get; set; }
    public string? Synopsis { get; set; }
    public string? Description { get; set; }
    public int? SeriesNumber { get; set; }
    public Guid? SeriesId { get; set; }
}