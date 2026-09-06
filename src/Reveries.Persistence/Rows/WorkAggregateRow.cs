namespace Reveries.Persistence.Rows;

public sealed class WorkAggregateRow
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string? Subtitle { get; init; }
    public string? Synopsis { get; init; }
    public string? Description { get; init; }
    public int? SeriesNumber { get; init; }

    public Guid? SeriesId { get; init; }
    public string? SeriesName { get; init; }

    public string PrimaryGenres { get; init; } = "[]";
    public string SecondaryGenres { get; init; } = "[]";
    public string Authors { get; init; } = "[]";

    public string[] DeweyCodes { get; init; } = [];
}