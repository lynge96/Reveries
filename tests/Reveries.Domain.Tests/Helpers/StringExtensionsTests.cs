using Reveries.Domain.Helpers;

namespace Reveries.Domain.Tests.Helpers;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("penguin books", "Penguin Books")]
    [InlineData("SIMON AND SCHUSTER", "Simon And Schuster")]
    [InlineData("", "")]
    public void ToTitleCase_NormalizesCasing(string input, string expected)
    {
        Assert.Equal(expected, input.ToTitleCase());
    }
}