using System.ComponentModel.DataAnnotations;

namespace Reveries.Api.Contracts.Books.Requests;

public sealed record SetBookSeriesRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public required string SeriesName { get; init; }

    [Range(1, int.MaxValue)]
    public int? NumberInSeries { get; init; }
}
