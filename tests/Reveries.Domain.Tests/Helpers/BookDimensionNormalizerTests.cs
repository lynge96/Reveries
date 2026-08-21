using Reveries.Domain.Helpers;

namespace Reveries.Domain.Tests.Helpers;

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

    [Fact]
    public void OrderDimensionsBySize_WithMillimetersStraddlingThreshold_ConvertsWholeSetConsistently()
    {
        var result = BookDimensionNormalizer.OrderDimensionsBySize(200, 130, 20);

        Assert.Equal(20, result.Height);
        Assert.Equal(13, result.Width);
        Assert.Equal(2, result.Thickness);
    }

    [Fact]
    public void OrderDimensionsBySize_WithCentimeters_LeavesValuesUnscaled()
    {
        var result = BookDimensionNormalizer.OrderDimensionsBySize(24.5m, 15.2m, 2.1m);

        Assert.Equal(24.5m, result.Height);
        Assert.Equal(15.2m, result.Width);
        Assert.Equal(2.1m, result.Thickness);
    }
}
