using Reveries.Core.Helpers;

namespace Reveries.Core.Tests.Helpers;

public class StringExtensionsTests
{
    [Fact]
    public void GetLanguageName_WithEmptyString_ReturnsUnknown()
    {
        var result = string.Empty.GetLanguageName();

        Assert.Equal("Unknown", result);
    }
    
    [Theory]
    [InlineData("en", "English")]
    [InlineData("da", "Danish")]
    [InlineData("de", "German")]
    [InlineData("fr", "French")]
    public void GetLanguageName_WithValidIso639Code_ReturnsLanguageName(
        string languageCode, string expectedLanguageName)
    {
        var result = languageCode.GetLanguageName();

        Assert.Equal(expectedLanguageName, result);
    }
    
    [Theory]
    [InlineData("EN")]
    [InlineData("En")]
    [InlineData("eN")]
    public void GetLanguageName_WithUppercaseCode_ReturnsEnglish(string languageCode)
    {
        var result = languageCode.GetLanguageName();

        Assert.Equal("English", result);
    }
    
    
}