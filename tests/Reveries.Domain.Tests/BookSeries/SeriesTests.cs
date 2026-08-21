using Reveries.Domain.BookSeries;

namespace Reveries.Domain.Tests.BookSeries;

public class SeriesTests
{
    [Fact]
    public void Create_WithValidName_NormalizesAndHasNoId()
    {
        var series = Series.Create("the wheel of time");

        Assert.NotNull(series);
        Assert.Equal("The Wheel Of Time", series.Name);
    }

    [Fact]
    public void Reconstitute_CreatesFullyHydratedEntity()
    {
        var seriesId = SeriesId.New();

        var series = Series.Reconstitute(seriesId, "Stormlight Archive");

        Assert.Equal(seriesId, series.Id);
        Assert.Equal("Stormlight Archive", series.Name);
    }


}
