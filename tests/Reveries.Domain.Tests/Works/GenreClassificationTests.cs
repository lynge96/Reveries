using Reveries.Domain.Works;

namespace Reveries.Domain.Tests.Works;

public class GenreClassificationTests
{
    [Fact]
    public void Create_SeparatesPrimaryAndSecondary()
    {
        var classification = GenreClassification.Create(["Fiction", "History"], ["Fantasy", "Ancient"]);

        Assert.Equal(["Fiction", "History"], classification.Primary.Select(g => g.Name));
        Assert.Equal(["Fantasy", "Ancient"], classification.Secondary.Select(g => g.Name));
    }

    [Fact]
    public void Create_DeduplicatesWithinEachTier()
    {
        var classification = GenreClassification.Create(["Fiction", "fiction"], ["Fantasy", "fantasy"]);

        Assert.Single(classification.Primary);
        Assert.Single(classification.Secondary);
    }

    [Fact]
    public void Create_PrimaryWins_WhenValueAppearsInBothTiers()
    {
        var classification = GenreClassification.Create(["Fiction"], ["Fiction", "Fantasy"]);

        Assert.Equal(["Fiction"], classification.Primary.Select(g => g.Name));
        Assert.Equal(["Fantasy"], classification.Secondary.Select(g => g.Name));
    }

    [Fact]
    public void All_ReturnsPrimaryThenSecondary()
    {
        var classification = GenreClassification.Create(["Fiction"], ["Fantasy"]);

        Assert.Equal(["Fiction", "Fantasy"], classification.All.Select(g => g.Name));
    }

    [Fact]
    public void Empty_HasNoGenres()
    {
        Assert.Empty(GenreClassification.Empty.All);
    }
}