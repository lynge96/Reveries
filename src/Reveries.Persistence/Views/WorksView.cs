namespace Reveries.Persistence.Views;

public sealed class WorksView
{
    // Work
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string? Synopsis { get; init; }
    public string? Description { get; init; }
    public int? SeriesNumber { get; init; }
    public DateTimeOffset? DateCreatedWork { get; init; }

    // Series (null when the work has no series — LEFT JOIN)
    public Guid? SeriesId { get; init; }
    public string? SeriesName { get; init; }
    public DateTimeOffset? DateCreatedSeries { get; init; }

    // JSON fields
    public string PrimaryGenres { get; init; } = "[]";
    public string SecondaryGenres { get; init; } = "[]";
    public string Authors { get; init; } = "[]";

    // text[]
    public string[] DeweyCodes { get; init; } = [];
}