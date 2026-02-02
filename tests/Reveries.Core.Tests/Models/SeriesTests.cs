using Reveries.Core.Identity;
using Reveries.Core.Models;

namespace Reveries.Core.Tests.Models;

public class SeriesTests
{
    [Fact]
    public void Create_WithValidName_NormalizesAndHasNoId()
    {
        var series = Series.Create("the wheel of time");
        
        Assert.NotNull(series);
        Assert.Equal("The Wheel Of Time", series.Name);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithNullOrWhitespaceName_ReturnsSeriesWithNullName(string? input)
    {
        var series = Series.Create(input);
        
        Assert.NotNull(series);
        Assert.Null(series.Name);
    }
    
    [Fact]
    public void Reconstitute_CreatesFullyHydratedEntity()
    {
        var dateCreated = DateTimeOffset.UtcNow;
        var seriesId = SeriesId.New();

        var series = Series.Reconstitute(seriesId, "Stormlight Archive", dateCreated);
        
        Assert.Equal(seriesId, series.Id);
        Assert.Equal("Stormlight Archive", series.Name);
        Assert.Equal(dateCreated, series.DateCreated);
    }
    
    
}