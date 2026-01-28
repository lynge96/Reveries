using Reveries.Core.Models;

namespace Reveries.Core.Tests.Models;

public class SubjectTests
{
    [Fact]
    public void Create_WithValidGenre_NormalizesAndHasNoId()
    {
        var subject = Subject.Create("science fiction");
        
        Assert.NotNull(subject);
        Assert.Null(subject.Id);
        Assert.Equal("Science Fiction", subject.Genre);
    }
    
    [Fact]
    public void Reconstitute_CreatesFullyHydratedEntity()
    {
        var dateCreated = DateTimeOffset.UtcNow;
        
        var subject = Subject.Reconstitute(7, "History", dateCreated);

        Assert.Equal(7, subject.Id);
        Assert.Equal("History", subject.Genre);
        Assert.Equal(dateCreated, subject.DateCreated);
    }
    
    [Fact]
    public void WithId_AssignsIdAndPreservesState()
    {
        var original = Subject.Create("philosophy");

        var withId = original.WithId(3);

        Assert.Equal(3, withId.Id);
        Assert.Equal(original.Genre, withId.Genre);
        Assert.Equal(original.DateCreated, withId.DateCreated);
    }
    
}