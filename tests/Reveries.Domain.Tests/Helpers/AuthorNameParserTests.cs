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

    [Fact]
    public void Canonicalize_PreservesCasing()
    {
        Assert.Equal("Frank Herbert", AuthorNameNormalizer.Canonicalize("Frank Herbert"));
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
}
