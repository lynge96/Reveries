using Reveries.Core.Helpers;

namespace Reveries.Core.Tests.Helpers;

public class AuthorNameParserTests
{
    [Fact]
    public void Parse_WithNullInput_ReturnsEmptyTuple()
    {
        var result = AuthorNameNormalizer.Parse(null!);

        Assert.Equal(string.Empty, result.FirstName);
        Assert.Equal(string.Empty, result.LastName);
    }
    
    [Fact]
    public void Parse_WithCommaFormat_ParsesCorrectly()
    {
        var result = AuthorNameNormalizer.Parse("Smith, John");

        Assert.Equal("John", result.FirstName);
        Assert.Equal("Smith", result.LastName);
    }
    
    [Fact]
    public void Parse_WithTwoNames_FirstAndLast()
    {
        var result = AuthorNameNormalizer.Parse("Stephen King");

        Assert.Equal("Stephen", result.FirstName);
        Assert.Equal("King", result.LastName);
    }
    
    [Fact]
    public void Parse_WithCommaFormatAndMiddleName_IncludesMiddleNameInFirstName()
    {
        var result = AuthorNameNormalizer.Parse("Tolkien, John Ronald Reuel");

        Assert.Equal("John Ronald Reuel", result.FirstName);
        Assert.Equal("Tolkien", result.LastName);
    }
    
    [Fact]
    public void Parse_WithThreeNames_FirstMiddleLast()
    {
        var result = AuthorNameNormalizer.Parse("George Raymond Richard Martin");

        Assert.Equal("George Raymond Richard", result.FirstName);
        Assert.Equal("Martin", result.LastName);
    }
    
    [Fact]
    public void Parse_WithCommaAndExtraSpaces_TrimsCorrectly()
    {
        var result = AuthorNameNormalizer.Parse("  Smith  ,  John  ");

        Assert.Equal("John", result.FirstName);
        Assert.Equal("Smith", result.LastName);
    }
    
    [Theory]
    [InlineData("J.R.R. Tolkien", "J.R.R.", "Tolkien")]
    [InlineData("J. R. R. Martin", "J. R. R.", "Martin")]
    [InlineData("G.R.R. Martin", "G.R.R.", "Martin")]
    [InlineData("J.K. Rowling", "J.K.", "Rowling")]
    [InlineData("Flea", "Flea", "")]
    public void Parse_WithInitials_ParsesCorrectly(string input, string expectedFirst, string expectedLast)
    {
        var result = AuthorNameNormalizer.Parse(input);
    
        Assert.Equal(expectedFirst, result.FirstName);
        Assert.Equal(expectedLast, result.LastName);
    }
    
    [Fact]
    public void Parse_WithHyphenatedName_TreatsAsOneWord()
    {
        var result = AuthorNameNormalizer.Parse("Mary-Jane Watson");

        Assert.Equal("Mary-Jane", result.FirstName);
        Assert.Equal("Watson", result.LastName);
    }
    
}