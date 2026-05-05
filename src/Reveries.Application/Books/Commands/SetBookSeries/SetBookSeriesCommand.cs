using Mediator;
using Reveries.Core.Identity;
using Reveries.Core.ValueObjects;

namespace Reveries.Application.Books.Commands.SetBookSeries;

public sealed record SetBookSeriesCommand : IQuery<BookId>
{
    public Isbn? Isbn { get; init; }
    public required string SeriesName { get; init; }
    public int? NumberInSeries { get; init; }
}