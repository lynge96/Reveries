using Reveries.Domain.Helpers;

namespace Reveries.Domain.Tests.Helpers;

public class AuthorNameNormalizerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Canonicalize_WithEmptyInput_ReturnsEmpty(string? input)
    {
        Assert.Equal(string.Empty, AuthorNameNormalizer.Canonicalize(input));
    }

    [Fact]
    public void Canonicalize_WithNaturalOrder_PassesThrough()
    {
        Assert.Equal("Stephen King", AuthorNameNormalizer.Canonicalize("Stephen King"));
    }

    [Theory]
    [InlineData("frank herbert", "Frank Herbert")]
    [InlineData("bell hooks", "Bell Hooks")]
    public void Canonicalize_CapitalizesFirstLetterOfEachName(string input, string expected)
    {
        Assert.Equal(expected, AuthorNameNormalizer.Canonicalize(input));
    }

    [Theory]
    [InlineData("Cormac McCarthy", "Cormac McCarthy")]
    [InlineData("Don DeLillo", "Don DeLillo")]
    public void Canonicalize_PreservesInteriorCapitals(string input, string expected)
    {
        Assert.Equal(expected, AuthorNameNormalizer.Canonicalize(input));
    }

    [Fact]
    public void Canonicalize_CollapsesWhitespace()
    {
        Assert.Equal("Frank Herbert", AuthorNameNormalizer.Canonicalize("  Frank   Herbert  "));
    }

    [Fact]
    public void Canonicalize_StripsSpecialCharacters()
    {
        Assert.Equal("Frank Herbert", AuthorNameNormalizer.Canonicalize("Frank Herbert #1"));
    }

    [Theory]
    [InlineData("O’Brien", "O'Brien")]
    [InlineData("D‘Angelo", "D'Angelo")]
    public void Canonicalize_NormalizesTypographicApostrophes(string input, string expected)
    {
        Assert.Equal(expected, AuthorNameNormalizer.Canonicalize(input));
    }

    [Theory]
    [InlineData("Frank Herbert.", "Frank Herbert")]
    [InlineData("-Frank Herbert", "Frank Herbert")]
    public void Canonicalize_TrimsDanglingSeparators(string input, string expected)
    {
        Assert.Equal(expected, AuthorNameNormalizer.Canonicalize(input));
    }

    [Fact]
    public void Canonicalize_WithCommaForm_ReordersToNaturalOrder()
    {
        Assert.Equal("Frank Herbert", AuthorNameNormalizer.Canonicalize("Herbert, Frank"));
    }

    [Fact]
    public void Canonicalize_WithCommaFormAndMiddleNames_ReordersToNaturalOrder()
    {
        Assert.Equal(
            "John Ronald Reuel Tolkien",
            AuthorNameNormalizer.Canonicalize("Tolkien, John Ronald Reuel"));
    }

    [Fact]
    public void Canonicalize_WithCommaFormAndSuffix_ReordersWithoutResidualComma()
    {
        Assert.Equal(
            "Martin Luther Jr. King",
            AuthorNameNormalizer.Canonicalize("King, Martin Luther, Jr."));
    }
}
