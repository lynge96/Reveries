using Reveries.Domain.Authors;

namespace Reveries.Domain.Tests.Authors;

public class AuthorTests
{
    [Fact]
    public void TryCreate_WithValidName_SetsNameAndNormalizedName()
    {
        var author = Author.TryCreate("Jane Austen");

        Assert.NotNull(author);
        Assert.Equal("Jane Austen", author!.Name);
        Assert.Equal("jane austen", author.NormalizedName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryCreate_WithEmptyName_ReturnsNull(string? name)
    {
        Assert.Null(Author.TryCreate(name));
    }

    [Fact]
    public void Reconstitute_RestoresAuthorState()
    {
        var date = DateTimeOffset.UtcNow;
        var authorId = AuthorId.New();

        var author = Author.Reconstitute(
            id: authorId,
            name: "Jane Austen",
            dateCreated: date
        );

        Assert.Equal(authorId, author.Id);
        Assert.Equal("Jane Austen", author.Name);
        Assert.Equal("jane austen", author.NormalizedName);
        Assert.Equal(date, author.DateCreated);
    }
}
