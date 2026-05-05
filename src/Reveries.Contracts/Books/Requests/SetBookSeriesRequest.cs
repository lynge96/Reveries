namespace Reveries.Contracts.Books.Requests;

public sealed record SetBookSeriesRequest
{
    public required string SeriesName { get; init; }
    public int? NumberInSeries { get; init; }
}