using Reveries.Domain.Works;

namespace Reveries.Domain.Tests.Works;

public class GenreTests
{
    [Fact]
    public void TryCreate_WithValidName_TitleCasesAndTrims()
    {
        var genre = Genre.TryCreate("  science fiction  ");

        Assert.NotNull(genre);
        Assert.Equal("Science Fiction", genre!.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryCreate_WithEmptyName_ReturnsNull(string? name)
    {
        Assert.Null(Genre.TryCreate(name));
    }
}