using Reveries.Core.Helpers;

namespace Reveries.Core.Tests.Helpers;

public class DeweyDecimalParserTests
{
    [Fact]
    public void NormalizeDeweyDecimals_WithNullInput_ReturnsNull()
    {
        var result = ((IEnumerable<string>?)null).NormalizeDeweyDecimals();

        Assert.Null(result);
    }
    
    [Fact]
    public void NormalizeDeweyDecimals_WithEmptyCollection_ReturnsNull()
    {
        var result = new List<string>().NormalizeDeweyDecimals();

        Assert.Null(result);
    }
    
    [Theory]
    [InlineData("100")]
    [InlineData("200.5")]
    [InlineData("300.123")]
    [InlineData("813")]
    public void NormalizeDeweyDecimals_WithStandardFormat_ReturnsListWithCode(string code)
    {
        var result = new[] { code }.NormalizeDeweyDecimals();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(code, result[0].Code);
    }
    
    [Theory]
    [InlineData("813/.6", "813.6")]
    [InlineData("100/.", "100")]
    [InlineData("787.2/.3", "787.2.3")]
    [InlineData("200/.1/.2", "200.1.2")]
    public void NormalizeDeweyDecimals_WithSlashDecimalFormat_ReplacesSlashWithDecimal(
        string input, string expected)
    {
        var result = new[] { input }.NormalizeDeweyDecimals();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(expected, result[0].Code);
    }
    
    [Theory]
    [InlineData("787.87/166092 B", "787.87")]
    [InlineData("100/500", "100")]
    [InlineData("200.5/999", "200.5")]
    [InlineData("813/1234567", "813")]
    public void NormalizeDeweyDecimals_WithSlashNumberFormat_RemovesSlashAndAfter(
        string input, string expected)
    {
        var result = new[] { input }.NormalizeDeweyDecimals();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(expected, result[0].Code);
    }
    
    [Fact]
    public void NormalizeDeweyDecimals_WithDuplicates_RemovesDuplicates()
    {
        var input = new[] { "100", "100", "200" };
        var result = input.NormalizeDeweyDecimals();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Single(result, r => r.Code == "100");
    }
}