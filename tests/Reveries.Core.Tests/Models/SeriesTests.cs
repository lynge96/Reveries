using Reveries.Core.Models;

namespace Reveries.Core.Tests.Models;

public class SeriesTests
{
    [Fact]
    public void Create_WithValidName_NormalizesAndHasNoId()
    {
        var series = Series.Create("the wheel of time");
        
        Assert.NotNull(series);
        Assert.Null(series.Id);
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
        Assert.Null(series.Id);
    }
    
    [Fact]
    public void Reconstitute_CreatesFullyHydratedEntity()
    {
        var dateCreated = DateTimeOffset.UtcNow;

        var series = Series.Reconstitute(42, "Stormlight Archive", dateCreated);
        
        Assert.Equal(42, series.Id);
        Assert.Equal("Stormlight Archive", series.Name);
        Assert.Equal(dateCreated, series.DateCreated);
    }
    
    [Fact]
    public void WithId_AssignsIdAndPreservesState()
    {
        var original = Series.Create("Malazan Book of the Fallen");

        var withId = original.WithId(10);

        Assert.Equal(10, withId.Id);
        Assert.Equal(original.Name, withId.Name);
        Assert.Equal(original.DateCreated, withId.DateCreated);
    }
    
    
}