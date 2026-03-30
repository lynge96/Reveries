using MediatR;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Commands.SetBookSeries;

public sealed record SetBookSeriesCommand : IRequest<int>
{
    public Isbn? Isbn { get; init; }
    public required string SeriesName { get; init; }
    public int? NumberInSeries { get; init; }
}