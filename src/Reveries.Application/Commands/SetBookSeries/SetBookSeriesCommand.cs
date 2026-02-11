using Reveries.Application.Commands.Abstractions;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Commands.SetBookSeries;

public sealed record SetBookSeriesCommand : ICommand
{
    public Isbn? Isbn { get; init; }
    public required string SeriesName { get; init; }
    public int? NumberInSeries { get; init; }
}