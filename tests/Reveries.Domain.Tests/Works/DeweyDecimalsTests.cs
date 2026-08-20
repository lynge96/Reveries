using Reveries.Domain.Enums;
using Reveries.Domain.Works;

namespace Reveries.Domain.Tests.Works;

public class DeweyDecimalsTests
{
    [Theory]
    [InlineData("813/.6", "813.6")]
    [InlineData("787.87/166092", "787.87")]
    [InlineData("100.", "100")]
    [InlineData(" 200.5 ", "200.5")]
    [InlineData("300.7/12", "300.7")]
    [InlineData("400", "400")]
    public void TryCreate_NormalizesValidCodes(string input, string expected)
    {
        var dewey = DeweyDecimal.TryCreate(input);

        Assert.NotNull(dewey);
        Assert.Equal(expected, dewey!.Code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(".")]
    [InlineData("Fic")]
    [InlineData("[E]")]
    public void TryCreate_ReturnsNull_ForInvalidInput(string? input)
    {
        var dewey = DeweyDecimal.TryCreate(input);

        Assert.Null(dewey);
    }

    [Theory]
    [InlineData("005.1", DeweyClass.General)]
    [InlineData("150", DeweyClass.Philosophy)]
    [InlineData("220", DeweyClass.Religion)]
    [InlineData("320", DeweyClass.SocialSciences)]
    [InlineData("440", DeweyClass.Language)]
    [InlineData("510", DeweyClass.Science)]
    [InlineData("620", DeweyClass.Technology)]
    [InlineData("750", DeweyClass.Arts)]
    [InlineData("813.54", DeweyClass.Literature)]
    [InlineData("940", DeweyClass.History)]
    public void MainClass_MapsFirstDigit(string input, DeweyClass expected)
    {
        var dewey = DeweyDecimal.TryCreate(input);

        Assert.Equal(expected, dewey!.MainCategory);
    }

    [Theory]
    [InlineData("005", "Computer science, information & general works")]
    [InlineData("813.54", "Literature")]
    [InlineData("940", "History & geography")]
    public void MainClassName_ReturnsDdc23Label(string input, string expected)
    {
        var dewey = DeweyDecimal.TryCreate(input);

        Assert.Equal(expected, dewey!.MainCategoryName);
    }

    [Theory]
    [InlineData("8ab")]
    [InlineData("5xx")]
    [InlineData("1234")]
    [InlineData("813.54.2")]
    [InlineData("12+34")]
    public void TryCreate_ReturnsNull_ForMalformedCodes(string input)
    {
        var dewey = DeweyDecimal.TryCreate(input);

        Assert.Null(dewey);
    }

    [Fact]
    public void ToString_ReturnsCode()
    {
        var dewey = DeweyDecimal.TryCreate("813.54");

        Assert.Equal("813.54", dewey!.ToString());
    }
}