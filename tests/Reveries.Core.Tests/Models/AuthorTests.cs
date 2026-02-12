using Reveries.Core.Identity;
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
    public void AddNameVariant_WithNewVariant_AddsVariant()
    {
        var author = Author.Create("Jane Austen");

        author.AddNameVariant("J. Austen", false);

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
        var authorId = AuthorId.New();

        var author = Author.Reconstitute(
            id: authorId,
            firstName: "Jane",
            lastName: "Austen",
            dateCreated: date
        );

        Assert.Equal(authorId, author.Id);
        Assert.Equal("jane austen", author.NormalizedName);
        Assert.Equal("Jane", author.FirstName);
        Assert.Equal("Austen", author.LastName);
        Assert.Equal(date, author.DateCreated);
    }
    
    [Fact]
    public void AddNameVariant_WhenNewPrimary_IsAdded_RemovesPreviousPrimary()
    {
        var author = Author.Create("Jane Austen");

        author.AddNameVariant("J. Austen", true);
        author.AddNameVariant("Jane A.", true);
        
        Assert.Equal(2, author.NameVariants.Count);

        var primaryVariants = author.NameVariants.Where(v => v.IsPrimary).ToList();

        Assert.Single(primaryVariants);
        Assert.Equal("Jane A.", primaryVariants[0].NameVariant);
    }
    
    [Fact]
    public void AddNameVariant_WhenNewVariantIsNotPrimary_DoesNotChangeExistingPrimary()
    {
        var author = Author.Create("Jane Austen");

        author.AddNameVariant("J. Austen", true);
        author.AddNameVariant("Jane A.", false);

        var primary = author.NameVariants.Single(v => v.IsPrimary);

        Assert.Equal("J. Austen", primary.NameVariant);
    }
    
    
    
}