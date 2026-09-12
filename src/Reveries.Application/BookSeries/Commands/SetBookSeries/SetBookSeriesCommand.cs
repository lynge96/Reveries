using Mediator;
using Reveries.Domain.Editions;
using Reveries.Domain.Works;

namespace Reveries.Application.BookSeries.Commands.SetBookSeries;

public sealed record SetBookSeriesCommand : ICommand<WorkId>
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
