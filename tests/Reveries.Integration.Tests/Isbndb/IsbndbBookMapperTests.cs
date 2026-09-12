using Reveries.Integration.Isbndb.Dtos;
using Reveries.Integration.Isbndb.Mappers;

namespace Reveries.Integration.Tests.Isbndb;

public class IsbndbBookMapperTests
{
    [Fact]
    public void ToBookCandidate_WithoutIsbn_ReturnsNull()
    {
        var dto = new IsbndbBookDto { Title = "No ISBN" };

        Assert.Null(dto.ToBookCandidate());
    }

    [Fact]
    public void ToBookCandidate_MapsCoreFields()
    {
        var dto = new IsbndbBookDto
        {
            Isbn13 = "9780132350884",
            Title = "Clean Code",
            Authors = ["Robert C. Martin"],
            Publisher = "Prentice Hall",
            Subjects = ["Software Engineering"],
            Pages = 464,
        };

        var candidate = dto.ToBookCandidate();

        Assert.NotNull(candidate);
        Assert.Equal("9780132350884", candidate.Isbn?.Value13);
        Assert.Equal("Clean Code", candidate.Title);
        Assert.Contains("Robert C. Martin", candidate.Authors);
        Assert.Equal(464, candidate.Pages);
        Assert.Contains("Software Engineering", candidate.SecondaryGenres);
    }

    [Fact]
    public void ToBookCandidate_FallsBackToIsbn10_WhenNoIsbn13()
    {
        var dto = new IsbndbBookDto
        {
            Isbn10 = "0132350882",
            Title = "Clean Code",
        };

        var candidate = dto.ToBookCandidate();

        Assert.NotNull(candidate);
        Assert.NotNull(candidate.Isbn);
    }

    [Fact]
    public void ToBookCandidate_ConvertsLengthFromInchesToCentimeters()
    {
        var dto = new IsbndbBookDto
        {
            Isbn13 = "9780132350884",
            Title = "Clean Code",
            DimensionsStructured = new IsbndbDimensionsDto
            {
                Height = new IsbndbDimensionDto { Unit = "inches", Value = 8.5 },
            },
        };

        var candidate = dto.ToBookCandidate();

        Assert.NotNull(candidate?.Dimensions);
        Assert.Equal(21.6m, candidate.Dimensions.HeightCm);
    }

    [Fact]
    public void ToBookCandidate_ConvertsWeightFromPoundsToGrams()
    {
        var dto = new IsbndbBookDto
        {
            Isbn13 = "9780132350884",
            Title = "Clean Code",
            DimensionsStructured = new IsbndbDimensionsDto
            {
                Height = new IsbndbDimensionDto { Unit = "centimeters", Value = 23.5 },
                Weight = new IsbndbDimensionDto { Unit = "pounds", Value = 1.2 },
            },
        };

        var candidate = dto.ToBookCandidate();

        Assert.NotNull(candidate?.Dimensions);
        Assert.Equal(544m, candidate.Dimensions.WeightG);
    }
}