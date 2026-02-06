using Reveries.Core.Identity;
using Reveries.Core.Models;

namespace Reveries.Application.Commands.SetBookSeries;

public sealed record SetBookSeriesCommand()
{
    public required Book Book { get; init; }
    public required string Name { get; init; }
    public int? NumberInSeries { get; init; }
}