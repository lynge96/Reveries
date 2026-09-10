using Reveries.Integration.GoogleBooks.DTOs;
using Reveries.Integration.GoogleBooks.Mappers;

namespace Reveries.Integration.Tests.GoogleBooks;

public class GoogleBookMapperTests
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