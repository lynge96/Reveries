using Mediator;
using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Commands.SetBookSeries;

public sealed record SetBookSeriesCommand : IQuery<WorkId>
{
    public Isbn Isbn { get; }
    public string SeriesName { get; }
    public int? NumberInSeries { get; }

    public SetBookSeriesCommand(string isbn, string seriesName, int? numberInSeries)
    {
        Isbn = Isbn.Create(isbn);
        SeriesName = seriesName;
        NumberInSeries = numberInSeries;
    }
}
