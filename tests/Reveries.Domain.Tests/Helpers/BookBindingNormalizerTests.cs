using Reveries.Domain.Enums;
using Reveries.Domain.Helpers;

namespace Reveries.Domain.Tests.Helpers;

public class BookBindingNormalizerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("some obscure format")]
    public void GetStandardBinding_WithNullEmptyOrUnrecognized_ReturnsUnknown(string? input)
    {
        Assert.Equal(BookFormat.Unknown, input.GetStandardBinding());
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
    public void GetStandardBinding_WithPaperbackVariants_ReturnsPaperback(string input)
    {
        Assert.Equal(BookFormat.Paperback, input.GetStandardBinding());
    }

    [Theory]
    [InlineData("Hardcover")]
    [InlineData("HARDCOVER")]
    [InlineData("Hard Cover")]
    [InlineData("Hardback")]
    [InlineData("HB")]
    public void GetStandardBinding_WithHardbackVariants_ReturnsHardback(string input)
    {
        Assert.Equal(BookFormat.Hardback, input.GetStandardBinding());
    }

    [Theory]
    [InlineData("Ebook")]
    [InlineData("e-book")]
    [InlineData("Kindle")]
    [InlineData("Kindle Edition")]
    [InlineData("ePub")]
    public void GetStandardBinding_WithEbookVariants_ReturnsEbook(string input)
    {
        Assert.Equal(BookFormat.Ebook, input.GetStandardBinding());
    }

    [Theory]
    [InlineData("Audiobook")]
    [InlineData("Audio Book")]
    [InlineData("Audio CD")]
    [InlineData("Audible")]
    public void GetStandardBinding_WithAudiobookVariants_ReturnsAudiobook(string input)
    {
        Assert.Equal(BookFormat.Audiobook, input.GetStandardBinding());
    }
}