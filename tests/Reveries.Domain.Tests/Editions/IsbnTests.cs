using Reveries.Domain.Exceptions;
using Reveries.Domain.Editions;

namespace Reveries.Domain.Tests.Editions;

public class IsbnTests
{
    [Theory]
    [InlineData("9780306406157", "9780306406157", "0306406152")]
    [InlineData("978-0-306-40615-7", "9780306406157", "0306406152")]
    [InlineData("0-306-40615-2", "9780306406157", "0306406152")]
    [InlineData("059309932X", "9780593099322", "059309932X")]
    public void Create_WithValidIsbn_NormalizesAndDerivesBothForms(string input, string expectedIsbn13, string expectedIsbn10)
    {
        var isbn = Isbn.Create(input);

        Assert.Equal(expectedIsbn13, isbn.Value13);
        Assert.Equal(expectedIsbn10, isbn.Value10);
        Assert.Equal(expectedIsbn13, isbn.ToString());
    }

    [Fact]
    public void Create_With979Isbn13_HasNoIsbn10()
    {
        var isbn = Isbn.Create("9790123456785");

        Assert.Equal("9790123456785", isbn.Value13);
        Assert.Null(isbn.Value10);
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
