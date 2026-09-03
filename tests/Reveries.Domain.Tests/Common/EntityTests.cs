using Reveries.Domain.Authors;

namespace Reveries.Domain.Tests.Common;

public class EntityTests
{
    [Fact]
    public void Entities_WithSameId_AreEqual()
    {
        var id = AuthorId.New();
        var first = Author.Reconstitute(id, "Ursula K. Le Guin");
        var second = Author.Reconstitute(id, "A Different Spelling");

        Assert.True(first.Equals(second));
        Assert.True(first == second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Entities_WithDifferentIds_AreNotEqual()
    {
        var first = Author.Reconstitute(AuthorId.New(), "Ursula K. Le Guin");
        var second = Author.Reconstitute(AuthorId.New(), "Ursula K. Le Guin");

        Assert.False(first.Equals(second));
        Assert.True(first != second);
    }

    [Fact]
    public void Entities_DedupeById_InAHashSet()
    {
        var id = AuthorId.New();
        var set = new HashSet<Author>
        {
            Author.Reconstitute(id, "Ursula K. Le Guin"),
            Author.Reconstitute(id, "Ursula K. Le Guin")
        };

        Assert.Single(set);
    }

    [Fact]
    public void Entity_IsNotEqualToNull()
    {
        var author = Author.Reconstitute(AuthorId.New(), "Ursula K. Le Guin");

        Assert.False(author.Equals(null));
        Assert.True(author != null);
    }
}