using Reveries.Core.Models;

namespace Reveries.Core.Tests.Models;

public class DeweyDecimalsTests
{
    [Theory]
    [InlineData("813/.6", "813.6")]
    [InlineData("787.87/166092", "787.87")]
    [InlineData("100.", "100")]
    [InlineData(" 200.5 ", "200.5")]
    [InlineData("300.7/12", "300.7")]
    [InlineData("400", "400")]
    public void Create_NormalizesValidCodes(string input, string expected)
    {
        var dewey = DeweyDecimal.Create(input);
        
        Assert.NotNull(dewey);
        Assert.Equal(expected, dewey.Code);
    }

}