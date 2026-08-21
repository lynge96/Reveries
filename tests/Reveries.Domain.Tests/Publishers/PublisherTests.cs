using Reveries.Domain.Publishers;

namespace Reveries.Domain.Tests.Publishers;

public class PublisherTests
{
    [Fact]
    public void TryCreate_WithValidName_NormalizesName()
    {
        var publisher = Publisher.TryCreate("harper & row (U.S.A)");

        Assert.NotNull(publisher);
        Assert.Equal("Harper & Row", publisher!.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("(unknown)")]
    public void TryCreate_WithEmptyOrNoiseName_ReturnsNull(string? name)
    {
        Assert.Null(Publisher.TryCreate(name));
    }

    [Fact]
    public void Reconstitute_RestoresStateCorrectly()
    {
        var publisherId = PublisherId.New();

        var publisher = Publisher.Reconstitute(publisherId, "Penguin Books");

        Assert.Equal(publisherId, publisher.Id);
        Assert.Equal("Penguin Books", publisher.Name);
    }

    [Fact]
    public void ToString_ReturnsName()
    {
        var publisher = Publisher.TryCreate("harper & row");

        Assert.Equal("Harper & Row", publisher!.ToString());
    }
}