using Reveries.Domain.BookSeries;
using Reveries.Domain.Exceptions;
using Reveries.Domain.Works;

namespace Reveries.Domain.Tests.Works;

public class SeriesPlacementTests
{
    [Fact]
    public void Create_WithSeriesAndNumber_SetsBoth()
    {
        var series = Series.Create("Kaldet");

        var placement = SeriesPlacement.Create(series, 3);

        Assert.Equal(series, placement.Series);
        Assert.Equal(3, placement.Number);
    }

    [Fact]
    public void Create_WithoutNumber_LeavesNumberNull()
    {
        var series = Series.Create("Kaldet");

        var placement = SeriesPlacement.Create(series);

        Assert.Equal(series, placement.Series);
        Assert.Null(placement.Number);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithNonPositiveNumber_Throws(int number)
    {
        var series = Series.Create("Kaldet");

        Assert.Throws<InvalidSeriesNumberException>(() => SeriesPlacement.Create(series, number));
    }

    [Fact]
    public void Create_WithNullSeries_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => SeriesPlacement.Create(null!, 1));
    }
}