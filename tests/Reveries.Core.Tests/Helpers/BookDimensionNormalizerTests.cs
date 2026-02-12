using Reveries.Core.Helpers;

namespace Reveries.Core.Tests.Helpers;

public class BookDimensionNormalizerTests
{
    [Fact]
    public void OrderDimensionsBySize_WithAllNull_ReturnsAllNull()
    {
        var result = BookDimensionNormalizer.OrderDimensionsBySize(null, null, null);

        Assert.Null(result.Height);
        Assert.Null(result.Width);
        Assert.Null(result.Thickness);
    }
    
    [Fact]
    public void OrderDimensionsBySize_WithThreeValues_OrdersDescending()
    {
        var result = BookDimensionNormalizer.OrderDimensionsBySize(10, 20, 5);

        Assert.Equal(20, result.Height);
        Assert.Equal(10, result.Width);
        Assert.Equal(5, result.Thickness);
    }
    
    [Fact]
    public void OrderDimensionsBySize_WithMillimeters_ConvertsToCentimeters()
    {
        var result = BookDimensionNormalizer.OrderDimensionsBySize(200, 150, 100);

        Assert.Equal(20, result.Height);
        Assert.Equal(15, result.Width);
        Assert.Equal(10, result.Thickness);
    }
    
}