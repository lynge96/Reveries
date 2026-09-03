using Reveries.Domain.Editions;
using Reveries.Domain.Enums;

namespace Reveries.Domain.Tests.Editions;

public class PublicationDateTests
{
    [Fact]
    public void TryCreate_WithYearOnly_HasYearPrecision()
    {
        var date = PublicationDate.TryCreate("1949");

        Assert.NotNull(date);
        Assert.Equal(1949, date.Year);
        Assert.Null(date.Month);
        Assert.Null(date.Day);
        Assert.Equal(DatePrecision.Year, date.Precision);
        Assert.Equal("1949", date.Value);
    }

    [Fact]
    public void TryCreate_WithYearMonth_HasMonthPrecision()
    {
        var date = PublicationDate.TryCreate("1949-06");

        Assert.NotNull(date);
        Assert.Equal(6, date.Month);
        Assert.Null(date.Day);
        Assert.Equal(DatePrecision.Month, date.Precision);
        Assert.Equal("1949-06", date.Value);
    }

    [Fact]
    public void TryCreate_WithFullDate_HasDayPrecision()
    {
        var date = PublicationDate.TryCreate("1949-06-08");

        Assert.NotNull(date);
        Assert.Equal(8, date.Day);
        Assert.Equal(DatePrecision.Day, date.Precision);
        Assert.Equal("1949-06-08", date.Value);
    }

    [Fact]
    public void TryCreate_WithFullTimestamp_KeepsDatePartOnly()
    {
        var date = PublicationDate.TryCreate("2005-10-01T00:00:00");

        Assert.NotNull(date);
        Assert.Equal("2005-10-01", date.Value);
    }

    [Theory]
    [InlineData("1949-13", "1949")]
    [InlineData("2005-02-30", "2005-02")]
    public void TryCreate_WithInvalidComponent_DegradesToLowerPrecision(string input, string expectedValue)
    {
        var date = PublicationDate.TryCreate(input);

        Assert.NotNull(date);
        Assert.Equal(expectedValue, date.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("n/a")]
    [InlineData("9999")]
    public void TryCreate_WithUnparseableOrImplausibleInput_ReturnsNull(string? input)
    {
        Assert.Null(PublicationDate.TryCreate(input));
    }
}