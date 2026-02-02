using Reveries.Core.ValueObjects;

namespace Reveries.Core.Tests.ValueObjects;

public class GenreTests
{
    [Fact]
    public void Create_WithValidGenre_NormalizesAndHasNoId()
    {
        var genre = Genre.Create("science fiction");
        
        Assert.NotNull(genre);
        Assert.Equal("Science Fiction", genre.Value);
    }
    
}