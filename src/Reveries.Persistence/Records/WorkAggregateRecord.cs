namespace Reveries.Persistence.Records;

public sealed class WorkAggregateRecord
{
    public WorkRecord Work { get; set; } = null!;
    public SeriesRecord? Series { get; set; }
    public List<AuthorRecord> Authors { get; set; } = [];
    public List<GenreRecord> PrimaryGenres { get; set; } = [];
    public List<GenreRecord> SecondaryGenres { get; set; } = [];
    public List<DeweyDecimalRecord> DeweyDecimals { get; set; } = [];
}