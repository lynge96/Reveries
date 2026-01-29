using Reveries.Core.Exceptions;
using Reveries.Core.Validation;

namespace Reveries.Core.Tests.Validation;

public class IsbnValidatorTests
{
    [Theory]
    [InlineData("9780306406157", "9780306406157")]
    [InlineData("978-0-306-40615-7", "9780306406157")]
    [InlineData("0-306-40615-2", "0306406152")]
    [InlineData("059309932X", "059309932X")]
    public void NormalizeAndValidateOrThrow_WithValidIsbn_ReturnsNormalizedIsbn(
        string input,
        string expected)
    {
        var result = IsbnValidator.NormalizeAndValidateOrThrow(input);

        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NormalizeAndValidateOrThrow_WithNullOrWhitespace_ThrowsException(
        string? input)
    {
        var ex = Assert.Throws<InvalidIsbnException>(
            () => IsbnValidator.NormalizeAndValidateOrThrow(input));

        Assert.Contains("ISBN cannot be null or empty", ex.Message);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("123456789012")]
    [InlineData("abcdefghij")]
    [InlineData("9780306406158")] // Wrong checksum
    public void NormalizeAndValidateOrThrow_WithInvalidIsbn_ThrowsException(
        string input)
    {
        Assert.Throws<InvalidIsbnException>(
            () => IsbnValidator.NormalizeAndValidateOrThrow(input));
    }

}