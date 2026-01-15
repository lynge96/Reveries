using Reveries.Core.Enums;
using Reveries.Core.Helpers;

namespace Reveries.Core.Tests.Helpers;

public class BookBindingNormalizerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void GetStandardBinding_WithNullAndEmptyInput_ReturnsUnknown(string? input)
    {
        var result = input.GetStandardBinding();

        Assert.Equal(nameof(BindingType.Unknown), result);
    }
    
    [Theory]
    [InlineData("Paperback")]
    [InlineData("PAPERBACK")]
    [InlineData("Softcover")]
    [InlineData("Soft Cover")]
    [InlineData("Trade Paperback")]
    [InlineData("PB")]
    [InlineData("Mass Market Paperback")]
    [InlineData("TPB")]
    public void GetStandardBinding_WithPaperbackVariants_ReturnsPaperback(string input)
    {
        var result = input.GetStandardBinding();

        Assert.Equal(nameof(BindingType.Paperback), result);
    }
    
    [Theory]
    [InlineData("Hardcover")]
    [InlineData("HARDCOVER")]
    [InlineData("Hard Cover")]
    [InlineData("Hardback")]
    [InlineData("HB")]
    public void GetStandardBinding_WithHardbackVariants_ReturnsHardback(string input)
    {
        var result = input.GetStandardBinding();

        Assert.Equal(nameof(BindingType.Hardback), result);
    }
    
    [Fact]
    public void GetStandardBinding_ReturnsString()
    {
        var result = "paperback".GetStandardBinding();

        Assert.IsType<string>(result);
    }
    
    [Fact]
    public void GetStandardBinding_ReturnsNonNullString()
    {
        var result = "unknown-format".GetStandardBinding();

        Assert.NotNull(result);
    }
    
}