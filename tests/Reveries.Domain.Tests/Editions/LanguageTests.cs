using Reveries.Domain.Editions;

namespace Reveries.Domain.Tests.Editions;

public class LanguageTests
{
    [Theory]
    [InlineData("en", "en")]
    [InlineData("da", "da")]
    [InlineData("EN", "en")]
    [InlineData("en-US", "en")]
    [InlineData("pt-BR", "pt")]
    [InlineData("  de  ", "de")]
    public void TryCreate_NormalizesToIso639Code(string input, string expectedCode)
    {
        var language = Language.TryCreate(input);

        Assert.NotNull(language);
        Assert.Equal(expectedCode, language.Value);
    }

    [Theory]
    [InlineData("en", "English")]
    [InlineData("da", "Danish")]
    [InlineData("pt-BR", "Portuguese")]
    [InlineData("nn", "Norwegian Nynorsk")]
    public void DisplayName_ResolvesEnglishNameWithoutRegion(string input, string expectedName)
    {
        var language = Language.TryCreate(input);

        Assert.NotNull(language);
        Assert.Equal(expectedName, language.DisplayName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("zzz")]
    public void TryCreate_WithUnresolvableInput_ReturnsNull(string? input)
    {
        var language = Language.TryCreate(input);

        Assert.Null(language);
    }
}