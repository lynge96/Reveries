using NSubstitute;
using Reveries.Application.BookSeries.Services;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Interfaces.Repositories;

namespace Reveries.Application.Tests.BookSeries;

public class SeriesResolverTests
{
    private readonly ISeriesRepository _series = Substitute.For<ISeriesRepository>();

    [Fact]
    public async Task ResolveAsync_returns_the_existing_series_without_inserting()
    {
        var existing = Series.Create("Discworld");
        _series.GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(existing);

        var resolver = new SeriesResolver(_series);
        var result = await resolver.ResolveAsync(Series.Create("Discworld"));

        Assert.Same(existing, result);
        await _series.DidNotReceive().AddAsync(Arg.Any<Series>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResolveAsync_inserts_and_returns_the_incoming_series_when_absent()
    {
        _series.GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Series?)null);
        var incoming = Series.Create("Dune");

        var resolver = new SeriesResolver(_series);
        var result = await resolver.ResolveAsync(incoming);

        Assert.Same(incoming, result);
        await _series.Received(1).AddAsync(incoming, Arg.Any<CancellationToken>());
    }
}