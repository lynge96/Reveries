using Reveries.Domain.Helpers;

namespace Reveries.Domain.Tests.Helpers;

public class HtmlToPlainTextConverterTests
{
    [Fact]
    public void HtmlToPlainText_StripsTags()
    {
        Assert.Equal("Hello world", "<p>Hello <b>world</b></p>".HtmlToPlainText());
    }

    [Fact]
    public void HtmlToPlainText_ConvertsBreaksToNewlines()
    {
        Assert.Equal("Line one\nLine two", "Line one<br>Line two".HtmlToPlainText());
    }

    [Fact]
    public void HtmlToPlainText_DecodesHtmlEntities()
    {
        Assert.Equal("Tom & Jerry", "Tom &amp; Jerry".HtmlToPlainText());
    }

    [Fact]
    public void HtmlToPlainText_CollapsesExcessiveBlankLines()
    {
        Assert.Equal("A\n\nB", "A<br><br><br><br>B".HtmlToPlainText());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void HtmlToPlainText_WithBlankInput_ReturnsInput(string input)
    {
        Assert.Equal(input, input.HtmlToPlainText());
    }
}