using Reveries.Domain.Helpers;

namespace Reveries.Domain.Tests.Helpers;

public class LanguageResolverTests
{
    [Fact]
    public void GetLanguageName_WithEmptyString_ReturnsUnknown()
    {
        Assert.Equal("Unknown", string.Empty.GetLanguageName());
    }

    [Theory]
    [InlineData("en", "English")]
    [InlineData("da", "Danish")]
    [InlineData("de", "German")]
    [InlineData("fr", "French")]
    public void GetLanguageName_WithValidIso639Code_ReturnsLanguageName(
        string languageCode, string expectedLanguageName)
    {
        Assert.Equal(expectedLanguageName, languageCode.GetLanguageName());
    }

    [Theory]
    [InlineData("EN")]
    [InlineData("En")]
    [InlineData("eN")]
    public void GetLanguageName_WithUppercaseCode_ReturnsEnglish(string languageCode)
    {
        Assert.Equal("English", languageCode.GetLanguageName());
    }

    [Fact]
    public void GetLanguageName_WithRegionCode_DropsRegionParenthetical()
    {
        Assert.Equal("Portuguese", "pt-BR".GetLanguageName());
    }

    [Fact]
    public void GetLanguageName_WithMultiWordName_PreservesFullName()
    {
        Assert.Equal("Norwegian Nynorsk", "nn".GetLanguageName());
    }

    [Fact]
    public void GetLanguageName_WithUnknownCode_ReturnsInput()
    {
        Assert.Equal("zzz", "zzz".GetLanguageName());
    }
}