using Reveries.Domain.Works;

namespace Reveries.Domain.Tests.Works;

public class DescriptionTests
{
    [Fact]
    public void TryCreate_StripsHtmlTags()
    {
        var description = Description.TryCreate("<p><b>Hello</b> <i>world</i></p>");

        Assert.Equal("Hello world", description!.Text);
    }

    [Fact]
    public void TryCreate_DecodesHtmlEntities()
    {
        var description = Description.TryCreate("Stephen Fry&#39;s Odyssey &amp; more");

        Assert.Equal("Stephen Fry's Odyssey & more", description!.Text);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("<br><p></p>")]
    public void TryCreate_WithEmptyOrTagOnlyValue_ReturnsNull(string? value)
    {
        Assert.Null(Description.TryCreate(value));
    }
}