using Reveries.Domain.Publishers;

namespace Reveries.Domain.Tests.Publishers;

public class PublisherTests
{
    [Fact]
    public void Create_WithValidName_NormalizesName()
    {
        var publisher = Publisher.Create("harper & row (U.S.A)");

        Assert.NotNull(publisher);
        Assert.Equal("Harper & Row", publisher.Name);
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
    public void ToString_ReturnsTitleCaseName()
    {
        var publisher = Publisher.Create("harper & row");

        var str = publisher.ToString();

        Assert.Equal("Harper & Row", str);
    }
}
