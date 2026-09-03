using Reveries.Domain.Helpers;

namespace Reveries.Domain.Tests.Helpers;

public class PublisherNameNormalizerTests
{
    [Fact]
    public void Normalize_WithEmptyString_ReturnsEmptyString()
    {
        var result = PublisherNameNormalizer.Normalize(string.Empty);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Normalize_WithNull_ReturnsEmptyString()
    {
        var result = PublisherNameNormalizer.Normalize(null);

        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("Penguin Books (2020)", "Penguin Books")]
    [InlineData("Harper & Row (U.S.A)", "Harper & Row")]
    [InlineData("Simon & Schuster [Publisher]", "Simon & Schuster")]
    [InlineData("Macmillan @2020", "Macmillan")]
    public void Normalize_WithParentheses_RemovesContent(string input, string expected)
    {
        var result = PublisherNameNormalizer.Normalize(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Normalize_WithParenthesesAtStart_RemovesContent()
    {
        var result = PublisherNameNormalizer.Normalize("(London) Penguin Books");

        Assert.Equal("Penguin Books", result);
    }

    [Theory]
    [InlineData("penguin books", "Penguin Books")]
    [InlineData("harper row", "Harper Row")]
    [InlineData("oxford university press", "Oxford University Press")]
    [InlineData("SIMON AND SCHUSTER", "Simon And Schuster")]
    public void Normalize_CapitalizesFirstLetterOfEachWord(string input, string expected)
    {
        var result = PublisherNameNormalizer.Normalize(input);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("HarperCollins", "HarperCollins")]
    [InlineData("McGraw-Hill", "McGraw-Hill")]
    [InlineData("O'Reilly Media", "O'Reilly Media")]
    public void Normalize_PreservesIntentionalBrandCasing(string input, string expected)
    {
        var result = PublisherNameNormalizer.Normalize(input);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Éditions Gallimard", "Éditions Gallimard")]
    [InlineData("Rowohlt & Möller", "Rowohlt & Möller")]
    [InlineData("Núñez", "Núñez")]
    public void Normalize_WithNonAsciiLetters_PreservesThem(string input, string expected)
    {
        var result = PublisherNameNormalizer.Normalize(input);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Penguin, ", "Penguin")]
    [InlineData("& Sons", "Sons")]
    [InlineData("'Penguin'", "Penguin")]
    public void Normalize_WithDanglingSeparators_TrimsThem(string input, string expected)
    {
        var result = PublisherNameNormalizer.Normalize(input);

        Assert.Equal(expected, result);
    }
}