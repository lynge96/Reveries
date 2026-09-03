using Reveries.Domain.Enums;
using Reveries.Domain.Helpers;

namespace Reveries.Domain.Tests.Helpers;

public class BookFormatNormalizerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("some obscure format")]
    public void GetStandardFormat_WithNullEmptyOrUnrecognized_ReturnsUnknown(string? input)
    {
        Assert.Equal(BookFormat.Unknown, input.GetStandardFormat());
    }

    [Theory]
    [InlineData("Paperback")]
    [InlineData("PAPERBACK")]
    [InlineData("Softcover")]
    [InlineData("Soft Cover")]
    [InlineData("Trade Paperback")]
    [InlineData("PB")]
    [InlineData("Mass Market Paperback")]
    [InlineData("TPB")]
    [InlineData("Perfect Paperback")]
    public void GetStandardFormat_WithPaperbackVariants_ReturnsPaperback(string input)
    {
        Assert.Equal(BookFormat.Paperback, input.GetStandardFormat());
    }

    [Theory]
    [InlineData("Hardcover")]
    [InlineData("HARDCOVER")]
    [InlineData("Hard Cover")]
    [InlineData("Hardback")]
    [InlineData("HB")]
    public void GetStandardFormat_WithHardbackVariants_ReturnsHardback(string input)
    {
        Assert.Equal(BookFormat.Hardback, input.GetStandardFormat());
    }

    [Theory]
    [InlineData("Ebook")]
    [InlineData("e-book")]
    [InlineData("Kindle")]
    [InlineData("Kindle Edition")]
    [InlineData("ePub")]
    public void GetStandardFormat_WithEbookVariants_ReturnsEbook(string input)
    {
        Assert.Equal(BookFormat.Ebook, input.GetStandardFormat());
    }

    [Theory]
    [InlineData("Audiobook")]
    [InlineData("Audio Book")]
    [InlineData("Audio CD")]
    [InlineData("Audible")]
    public void GetStandardFormat_WithAudiobookVariants_ReturnsAudiobook(string input)
    {
        Assert.Equal(BookFormat.Audiobook, input.GetStandardFormat());
    }
}
