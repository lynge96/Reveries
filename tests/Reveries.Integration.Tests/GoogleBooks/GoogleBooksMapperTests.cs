using Reveries.Integration.GoogleBooks.Dtos;
using Reveries.Integration.GoogleBooks.Mappers;

namespace Reveries.Integration.Tests.GoogleBooks;

public class GoogleBooksMapperTests
{
    [Fact]
    public void ToBookCandidate_WithoutIsbn_ReturnsNull()
    {
        var dto = new GoogleVolumeInfoDto { Title = "No ISBN" };

        Assert.Null(dto.ToBookCandidate());
    }

    [Fact]
    public void ToBookCandidate_ExtractsIsbn13AndSplitsCategories()
    {
        var dto = new GoogleVolumeInfoDto
        {
            Title = "Clean Code",
            IndustryIdentifiers =
            [
                new GoogleIndustryIdentifierDto { Type = "ISBN_13", Identifier = "9780132350884" },
            ],
            Categories = ["Fiction / Classics"],
        };

        var candidate = dto.ToBookCandidate();

        Assert.NotNull(candidate);
        Assert.Equal("9780132350884", candidate.Isbn?.Value13);
        Assert.Contains("Fiction", candidate.PrimaryGenres);
        Assert.Contains("Classics", candidate.SecondaryGenres);
    }

    [Fact]
    public void ToBookCandidate_FallsBackToIsbn10_WhenNoIsbn13()
    {
        var dto = new GoogleVolumeInfoDto
        {
            Title = "Clean Code",
            IndustryIdentifiers =
            [
                new GoogleIndustryIdentifierDto { Type = "ISBN_10", Identifier = "0132350882" },
            ],
        };

        var candidate = dto.ToBookCandidate();

        Assert.NotNull(candidate);
        Assert.NotNull(candidate.Isbn);
    }

    [Fact]
    public void ToBookCandidate_SplitsMultiSegmentCategoryIntoPrimaryAndSecondaries()
    {
        var dto = new GoogleVolumeInfoDto
        {
            Title = "The Hobbit",
            IndustryIdentifiers =
            [
                new GoogleIndustryIdentifierDto { Type = "ISBN_13", Identifier = "9780132350884" },
            ],
            Categories = ["Fiction / Fantasy / Epic"],
        };

        var candidate = dto.ToBookCandidate();

        Assert.NotNull(candidate);
        Assert.Contains("Fiction", candidate.PrimaryGenres);
        Assert.Contains("Fantasy", candidate.SecondaryGenres);
        Assert.Contains("Epic", candidate.SecondaryGenres);
    }

    [Fact]
    public void ToBookCandidate_ParsesDimensionsAndOrdersLargestAsHeight()
    {
        var dto = new GoogleVolumeInfoDto
        {
            Title = "Clean Code",
            IndustryIdentifiers =
            [
                new GoogleIndustryIdentifierDto { Type = "ISBN_13", Identifier = "9780132350884" },
            ],
            Dimensions = new GoogleDimensionsDto
            {
                Height = "24.00 cm",
                Width = "16.00 cm",
                Thickness = "3.00 cm",
            },
        };

        var candidate = dto.ToBookCandidate();

        Assert.NotNull(candidate?.Dimensions);
        Assert.Equal(24m, candidate.Dimensions.HeightCm);
        Assert.Equal(16m, candidate.Dimensions.WidthCm);
        Assert.Equal(3m, candidate.Dimensions.ThicknessCm);
    }

    [Fact]
    public void ToBookCandidate_MapsDescriptionIntoBothSynopsisAndDescription()
    {
        var dto = new GoogleVolumeInfoDto
        {
            Title = "Clean Code",
            IndustryIdentifiers =
            [
                new GoogleIndustryIdentifierDto { Type = "ISBN_13", Identifier = "9780132350884" },
            ],
            Description = "A handbook of agile software craftsmanship.",
        };

        var candidate = dto.ToBookCandidate();

        Assert.NotNull(candidate?.Synopsis);
        Assert.NotNull(candidate.Description);
        Assert.Equal(candidate.Synopsis, candidate.Description);
    }

    [Fact]
    public void ToBookCandidate_MapsThumbnailAsCoverAndSmallThumbnailAsThumbnail()
    {
        var dto = new GoogleVolumeInfoDto
        {
            Title = "Clean Code",
            IndustryIdentifiers =
            [
                new GoogleIndustryIdentifierDto { Type = "ISBN_13", Identifier = "9780132350884" },
            ],
            ImageLinks = new GoogleImageLinksDto
            {
                Thumbnail = "https://books.google.com/thumb.jpg",
                SmallThumbnail = "https://books.google.com/small.jpg",
            },
        };

        var candidate = dto.ToBookCandidate();

        Assert.Equal("https://books.google.com/thumb.jpg", candidate?.Cover?.Url);
        Assert.Equal("https://books.google.com/small.jpg", candidate?.Cover?.ThumbnailUrl);
    }
}