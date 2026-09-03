using Reveries.Domain.Helpers;

namespace Reveries.Domain.Tests.Helpers;

public class PageCountNormalizerTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(412)]
    [InlineData(50_000)]
    public void Normalize_WithReasonableCount_ReturnsIt(int pages)
    {
        Assert.Equal(pages, PageCountNormalizer.Normalize(pages));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-250)]
    public void Normalize_WithNullOrNonPositive_ReturnsNull(int? pages)
    {
        Assert.Null(PageCountNormalizer.Normalize(pages));
    }

    [Theory]
    [InlineData(50_001)]
    [InlineData(2_000_000)]
    public void Normalize_WithImplausiblyLargeCount_ReturnsNull(int pages)
    {
        Assert.Null(PageCountNormalizer.Normalize(pages));
    }
}