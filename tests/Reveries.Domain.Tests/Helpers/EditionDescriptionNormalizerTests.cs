using Reveries.Domain.Helpers;

namespace Reveries.Domain.Tests.Helpers;

public class EditionDescriptionNormalizerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\n")]
    public void Normalize_WithNullOrBlank_ReturnsNull(string? input)
    {
        Assert.Null(EditionDescriptionNormalizer.Normalize(input));
    }

    [Theory]
    [InlineData("  1st ed.  ", "1st ed.")]
    [InlineData("\tRevised Edition\n", "Revised Edition")]
    public void Normalize_TrimsOuterWhitespace(string input, string expected)
    {
        Assert.Equal(expected, EditionDescriptionNormalizer.Normalize(input));
    }

    [Theory]
    [InlineData("Revised    Edition", "Revised Edition")]
    [InlineData("Book\tClub\nEdition", "Book Club Edition")]
    public void Normalize_CollapsesInnerWhitespace(string input, string expected)
    {
        Assert.Equal(expected, EditionDescriptionNormalizer.Normalize(input));
    }

    [Fact]
    public void Normalize_StripsControlCharacters()
    {
        var withNull = "De" + (char)0 + "luxe Edition";
        Assert.Equal("Deluxe Edition", EditionDescriptionNormalizer.Normalize(withNull));

        var withBell = "First Edition" + (char)7;
        Assert.Equal("First Edition", EditionDescriptionNormalizer.Normalize(withBell));
    }

    [Theory]
    [InlineData("1st")]
    [InlineData("2nd Revised Edition")]
    [InlineData("Collector's Hardback Edition")]
    public void Normalize_PreservesDigitsAndPunctuation(string input)
    {
        Assert.Equal(input, EditionDescriptionNormalizer.Normalize(input));
    }
}
