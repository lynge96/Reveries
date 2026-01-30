using Reveries.Core.Exceptions;
using Reveries.Core.ValueObjects;

namespace Reveries.Core.Tests.ValueObjects;

public class IsbnTests
{
    [Theory]
    [InlineData("9780306406157", "9780306406157")]
    [InlineData("978-0-306-40615-7", "9780306406157")]
    [InlineData("0-306-40615-2", "0306406152")]
    [InlineData("059309932X", "059309932X")]
    public void Create_WithValidIsbn_ReturnsNormalizedIsbn(string input, string expected)
    {
        var isbn = Isbn.Create(input);

        Assert.Equal(expected, isbn.Value);
        Assert.Equal(expected, isbn.ToString());
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithNullOrWhitespace_ThrowsArgumentException(string? input)
    {
        var ex = Assert.Throws<InvalidIsbnException>(
            () => Isbn.Create(input!));

        Assert.Contains("ISBN cannot be null or empty", ex.Message);
    }
    
    [Theory]
    [InlineData("123")]
    [InlineData("123456789012")]
    [InlineData("abcdefghij")]
    [InlineData("9780306406158")] // Wrong checksum
    public void Create_WithInvalidIsbn_ThrowsArgumentException(string input)
    {
        Assert.Throws<InvalidIsbnException>(
            () => Isbn.Create(input));
    }
    
}