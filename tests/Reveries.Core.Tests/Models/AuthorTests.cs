using Reveries.Core.Models;

namespace Reveries.Core.Tests.Models;

public class AuthorTests
{
    [Fact]
    public void Create_WithValidName_CreatesAuthorWithNormalizedName()
    {
        // Act
        var author = Author.Create("Jane Austen");

        // Assert
        Assert.NotNull(author);
        Assert.Equal("jane austen", author.NormalizedName);
        Assert.Equal("Jane", author.FirstName);
        Assert.Equal("Austen", author.LastName);
        Assert.Empty(author.NameVariants);
    }
    
    [Fact]
    public void Create_WithEmptyName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Author.Create(""));
    }
    
    [Fact]
    public void AddNameVariant_WithNewVariant_AddsVariant()
    {
        var author = Author.Create("Jane Austen");

        author.AddNameVariant("J. Austen", isPrimary: false);

        Assert.Single(author.NameVariants);
        Assert.Equal("J. Austen", author.NameVariants[0].NameVariant);
        Assert.False(author.NameVariants[0].IsPrimary);
    }

    [Fact]
    public void AddNameVariant_DuplicateVariant_IsIgnored()
    {
        var author = Author.Create("Jane Austen");

        author.AddNameVariant("J. Austen", false);
        author.AddNameVariant("j. austen", true);

        Assert.Single(author.NameVariants);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddNameVariant_EmptyOrWhitespace_DoesNothing(string variant)
    {
        var author = Author.Create("Jane Austen");

        author.AddNameVariant(variant, false);

        Assert.Empty(author.NameVariants);
    }

    [Fact]
    public void Reconstitute_RestoresAuthorState()
    {
        var date = DateTimeOffset.UtcNow;

        var author = Author.Reconstitute(
            id: 42,
            normalizedName: "jane austen",
            firstName: "Jane",
            lastName: "Austen",
            dateCreated: date
        );

        Assert.Equal(42, author.Id);
        Assert.Equal("jane austen", author.NormalizedName);
        Assert.Equal("Jane", author.FirstName);
        Assert.Equal("Austen", author.LastName);
        Assert.Equal(date, author.DateCreated);
    }
    
}