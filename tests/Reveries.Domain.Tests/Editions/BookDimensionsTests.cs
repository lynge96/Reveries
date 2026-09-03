using Reveries.Domain.Editions;

namespace Reveries.Domain.Tests.Editions;

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

    [Fact]
    public void Reconstitute_PreservesStoredValuesVerbatim()
    {
        var dimensions = BookDimensions.Reconstitute(20.5m, 15.2m, 3m, 500m);

        Assert.NotNull(dimensions);
        Assert.Equal(20.5m, dimensions!.HeightCm);
        Assert.Equal(15.2m, dimensions.WidthCm);
        Assert.Equal(3m, dimensions.ThicknessCm);
        Assert.Equal(500m, dimensions.WeightG);
    }

    [Fact]
    public void Reconstitute_DoesNotReSanitize()
    {
        var dimensions = BookDimensions.Reconstitute(-10m, 0m, null, 42.7m);

        Assert.NotNull(dimensions);
        Assert.Equal(-10m, dimensions!.HeightCm);
        Assert.Equal(0m, dimensions.WidthCm);
        Assert.Null(dimensions.ThicknessCm);
        Assert.Equal(42.7m, dimensions.WeightG);
    }

    [Fact]
    public void Reconstitute_WithAllNull_ReturnsNull()
    {
        Assert.Null(BookDimensions.Reconstitute(null, null, null, null));
    }
}
