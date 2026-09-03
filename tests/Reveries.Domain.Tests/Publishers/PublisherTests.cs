using Reveries.Domain.Publishers;

namespace Reveries.Domain.Tests.Publishers;

public class PublisherTests
{
    [Theory]
    [InlineData("HarperCollins", "HarperCollins")]
    [InlineData("penguin books", "Penguin Books")]
    [InlineData("SIMON AND SCHUSTER", "Simon And Schuster")]
    public void TryCreate_KeepsDisplayCasingOnName(string input, string expectedName)
    {
        var publisher = Publisher.TryCreate(input);

        Assert.NotNull(publisher);
        Assert.Equal(expectedName, publisher.Name);
    }

    [Theory]
    [InlineData("HarperCollins", "harpercollins")]
    [InlineData("HARPERCOLLINS", "harpercollins")]
    [InlineData("harpercollins", "harpercollins")]
    public void TryCreate_LowercasesNormalizedNameSoCasingVariantsDedupe(string input, string expectedNormalized)
    {
        var publisher = Publisher.TryCreate(input);

        Assert.NotNull(publisher);
        Assert.Equal(expectedNormalized, publisher.NormalizedName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("()")]
    public void TryCreate_WithNothingMeaningful_ReturnsNull(string? input)
    {
        var publisher = Publisher.TryCreate(input);

        Assert.Null(publisher);
    }
}