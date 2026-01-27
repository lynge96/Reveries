using Reveries.Core.Models;

namespace Reveries.Core.Tests.Models;

public class BookDimensionsTests
{
    [Fact]
    public void Create_WithAllValidValues_ReturnsBookDimensions()
    {
        var dimensions = BookDimensions.Create(20m, 15m, 3m, 500m);
        
        Assert.NotNull(dimensions);
        Assert.Equal(20m, dimensions!.HeightCm);
        Assert.Equal(15m, dimensions.WidthCm);
        Assert.Equal(3m, dimensions.ThicknessCm);
        Assert.Equal(500m, dimensions.WeightG);
    }
    
    [Fact]
    public void Create_WithZeroOrNegativeValues_NormalizesToNull()
    {
        var dimensions = BookDimensions.Create(0m, -10m, 2m, -1m);
        
        Assert.NotNull(dimensions);
        Assert.Null(dimensions!.HeightCm);
        Assert.Null(dimensions.WidthCm);
        Assert.Equal(2m, dimensions.ThicknessCm);
        Assert.Null(dimensions.WeightG);
    }
    
    [Fact]
    public void Create_WithAllInvalidValues_ReturnsNull()
    {
        var dimensions = BookDimensions.Create(0m, -1m, 0m, -5m);
        
        Assert.Null(dimensions);
    }
    
    [Fact]
    public void Create_WithSomeNullValues_PreservesNulls()
    {
        var dimensions = BookDimensions.Create(null, 12m, null, 300m);
        
        Assert.NotNull(dimensions);
        Assert.Null(dimensions!.HeightCm);
        Assert.Equal(12m, dimensions.WidthCm);
        Assert.Null(dimensions.ThicknessCm);
        Assert.Equal(300m, dimensions.WeightG);
    }
    
}