using Reveries.Core.Identity;
using Reveries.Core.Models;

namespace Reveries.Core.Tests.Models;

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
        var date = DateTimeOffset.UtcNow;
        var publisherId = PublisherId.New();
        
        var publisher = Publisher.Reconstitute(publisherId, "Penguin Books", date);
        
        Assert.Equal(publisherId, publisher.Id);
        Assert.Equal("Penguin Books", publisher.Name);
        Assert.Equal(date, publisher.DateCreated);
    }

    [Fact]
    public void ToString_ReturnsTitleCaseName()
    {
        var publisher = Publisher.Create("harper & row");
        
        var str = publisher.ToString();

        Assert.Equal("Harper & Row", str);
    }
}