using Reveries.Domain.Editions;
using Reveries.Domain.Enums;
using Reveries.Domain.Exceptions;
using Reveries.Domain.Publishers;
using Reveries.Domain.Works;

namespace Reveries.Domain.Tests.Editions;

public class EditionTests
{
    private static Edition CreateValidEdition(
        string? isbn13 = "978-1-4028-9462-6",
        string? isbn10 = "1-4028-9462-7",
        int? pages = 412,
        string? languageIso639 = "en",
        string? format = "Hardcover",
        PublisherId? publisherId = null)
    {
        return Edition.Create(new EditionData(
            WorkId: WorkId.New(),
            Isbn13: isbn13,
            Isbn10: isbn10,
            PublisherId: publisherId,
            Pages: pages,
            PublishDate: "1965",
            LanguageIso639: languageIso639,
            Format: format,
            EditionStatement: "1st",
            ImageThumbnail: null,
            ImageUrl: null,
            SaxoUrl: null,
            Dimensions: null));
    }

    [Fact]
    public void Create_NormalizesIsbn()
    {
        var edition = CreateValidEdition();

        Assert.Equal("9781402894626", edition.Isbn?.Value13);
        Assert.Equal("1402894627", edition.Isbn?.Value10);
    }

    [Fact]
    public void Create_WithOnlyIsbn13_DoesNotThrow()
    {
        var edition = CreateValidEdition(isbn10: null);

        Assert.NotNull(edition.Isbn);
        Assert.Equal("9781402894626", edition.Isbn?.Value13);
    }

    [Fact]
    public void Create_WithOnlyIsbn10_DoesNotThrow()
    {
        var edition = CreateValidEdition(isbn13: null);

        Assert.NotNull(edition.Isbn);
        Assert.Equal("1402894627", edition.Isbn?.Value10);
    }

    [Fact]
    public void Create_AssignsWorkId()
    {
        var workId = WorkId.New();

        var edition = Edition.Create(new EditionData(
            WorkId: workId,
            Isbn13: "9781402894626",
            Isbn10: null,
            PublisherId: null,
            Pages: null,
            PublishDate: null,
            LanguageIso639: "en",
            Format: null,
            EditionStatement: null,
            ImageThumbnail: null,
            ImageUrl: null,
            SaxoUrl: null,
            Dimensions: null));

        Assert.Equal(workId, edition.WorkId);
    }

    [Fact]
    public void Create_AssignsPublisherId()
    {
        var publisherId = PublisherId.New();

        var edition = CreateValidEdition(publisherId: publisherId);

        Assert.Equal(publisherId, edition.PublisherId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-250)]
    public void Create_WithNonPositivePages_LeavesPagesNull(int? pageCount)
    {
        var edition = CreateValidEdition(pages: pageCount);

        Assert.Null(edition.Pages);
    }

    [Fact]
    public void Create_WithNullPages_LeavesPagesNull()
    {
        var edition = CreateValidEdition(pages: null);

        Assert.Null(edition.Pages);
    }

    [Fact]
    public void Create_NormalizesLanguageToIso639Code()
    {
        var edition = CreateValidEdition(languageIso639: "en-US");

        Assert.Equal("en", edition.Language?.Value);
        Assert.Equal("English", edition.Language?.DisplayName);
    }

    [Fact]
    public void Create_NormalizesFormat()
    {
        var edition = CreateValidEdition(format: "Hardcover");

        Assert.Equal(BookFormat.Hardback, edition.Format);
    }

    [Fact]
    public void SetPublisher_AssignsPublisherId()
    {
        var edition = CreateValidEdition();
        var publisherId = PublisherId.New();

        edition.SetPublisher(publisherId);

        Assert.Equal(publisherId, edition.PublisherId);
    }

    [Fact]
    public void Reconstitute_PreservesData()
    {
        var id = Guid.NewGuid();
        var workId = Guid.NewGuid();
        var publisherId = PublisherId.New();
        var data = new EditionReconstitutionData(
            Id: id,
            WorkId: workId,
            Isbn13: "9781402894626",
            Isbn10: "1402894627",
            Pages: 412,
            PublicationDate: "1965",
            Language: "en",
            EditionStatement: "1st",
            Format: BookFormat.Hardback,
            ImageThumbnailUrl: null,
            CoverImageUrl: null,
            SaxoUrl: null,
            Dimensions: null,
            PublisherId: publisherId);

        var edition = Edition.Reconstitute(data);

        Assert.Equal(id, edition.Id.Value);
        Assert.Equal(workId, edition.WorkId.Value);
        Assert.Equal("9781402894626", edition.Isbn?.Value13);
        Assert.Equal(412, edition.Pages);
        Assert.Equal("1965", edition.PublicationDate?.Value);
        Assert.Equal("en", edition.Language?.Value);
        Assert.Equal(publisherId, edition.PublisherId);
    }

    [Fact]
    public void Create_WithBothIsbnsNull_Throws()
    {
        Assert.Throws<MissingIsbnException>(() => CreateValidEdition(isbn13: null, isbn10: null));
    }
}