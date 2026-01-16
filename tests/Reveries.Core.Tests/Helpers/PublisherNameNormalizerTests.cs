using Reveries.Core.Helpers;

namespace Reveries.Core.Tests.Helpers;

public class PublisherNameNormalizerTests
{
    [Fact]
    public void Normalize_WithEmptyString_ReturnsEmptyString()
    {
        var result = string.Empty.StandardizePublisherName();

        Assert.Equal(string.Empty, result);
    }
    
    [Theory]
    [InlineData("Penguin Books (2020)", "Penguin Books")]
    [InlineData("Harper & Row (U.S.A)", "Harper & Row")]
    [InlineData("Simon & Schuster [Publisher]", "Simon & Schuster")]
    [InlineData("Macmillan @2020", "Macmillan")]
    public void Normalize_WithParentheses_RemovesContent(string input, string expected)
    {
        var result = input.StandardizePublisherName();

        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void Normalize_WithParenthesesAtStart_RemovesContent()
    {
        var result = "(London) Penguin Books".StandardizePublisherName();

        Assert.Equal("Penguin Books", result);
    }

    [Theory]
    [InlineData("penguin books", "Penguin Books")]
    [InlineData("SIMON AND SCHUSTER", "Simon And Schuster")]
    [InlineData("harper row", "Harper Row")]
    [InlineData("oxford university press", "Oxford University Press")]
    [InlineData("O'Reilly MEDIa", "O'reilly Media")]
    public void Normalize_WithLowercaseInput_ConvertsToTitleCase(string input, string expected)
    {
        var result = input.StandardizePublisherName();

        Assert.Equal(expected, result);
    }
    
    
}