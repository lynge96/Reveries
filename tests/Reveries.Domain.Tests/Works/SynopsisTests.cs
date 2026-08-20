using Reveries.Domain.Works;

namespace Reveries.Domain.Tests.Works;

public class SynopsisTests
{
    [Fact]
    public void TryCreate_StripsHtmlTags()
    {
        var synopsis = Synopsis.TryCreate("<p><b>Hello</b> <i>world</i></p>");

        Assert.Equal("Hello world", synopsis!.Text);
    }

    [Fact]
    public void TryCreate_DecodesHtmlEntities()
    {
        var synopsis = Synopsis.TryCreate("Stephen Fry&#39;s Odyssey &amp; more");

        Assert.Equal("Stephen Fry's Odyssey & more", synopsis!.Text);
    }

    [Fact]
    public void TryCreate_PreservesParagraphs_FromBreakTags()
    {
        var synopsis = Synopsis.TryCreate("First paragraph.<br> <br>Second paragraph.");

        Assert.Equal("First paragraph.\n\nSecond paragraph.", synopsis!.Text);
    }

    [Fact]
    public void TryCreate_NormalizesWhitespace()
    {
        var synopsis = Synopsis.TryCreate("  Lots   of    spaces  ");

        Assert.Equal("Lots of spaces", synopsis!.Text);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("<br><p></p>")]
    public void TryCreate_WithEmptyOrTagOnlyValue_ReturnsNull(string? value)
    {
        Assert.Null(Synopsis.TryCreate(value));
    }
}