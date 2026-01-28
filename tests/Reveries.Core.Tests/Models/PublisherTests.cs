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
    public void Create_WithNullOrWhitespace_ReturnsPublisherWithNullName()
    {
        var nullPublisher = Publisher.Create(null);
        var emptyPublisher = Publisher.Create("");
        var whitespacePublisher = Publisher.Create(" ");

        Assert.NotNull(nullPublisher);
        Assert.Null(nullPublisher.Name);
        Assert.NotNull(emptyPublisher);
        Assert.Null(emptyPublisher.Name);
        Assert.NotNull(whitespacePublisher);
        Assert.Null(whitespacePublisher.Name);
    }
    
    [Fact]
    public void Reconstitute_RestoresStateCorrectly()
    {
        var date = DateTimeOffset.UtcNow;
        var publisher = Publisher.Reconstitute(42, "Penguin Books", date);
        
        Assert.Equal(42, publisher.Id);
        Assert.Equal("Penguin Books", publisher.Name);
        Assert.Equal(date, publisher.DateCreated);
    }
    
    [Fact]
    public void WithId_ReturnsNewInstanceWithId()
    {
        var original = Publisher.Create("Random House");
        var withId = original.WithId(123);
        
        Assert.Equal(123, withId.Id);
        Assert.Equal(original.Name, withId.Name);
        
        Assert.Null(original.Id);
        Assert.Equal("Random House", original.Name);
    }

    [Fact]
    public void ToString_ReturnsTitleCaseName()
    {
        var publisher = Publisher.Create("harper & row");
        
        var str = publisher.ToString();

        Assert.Equal("Harper & Row", str);
    }
}