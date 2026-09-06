namespace Reveries.Persistence.Records;

public sealed class SeriesRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}