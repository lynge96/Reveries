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
        string? binding = "Hardcover")
    {
        return Edition.Create(new EditionData(
            WorkId: WorkId.New(),
            Isbn13: isbn13,
            Isbn10: isbn10,
            Publisher: "Chilton Books",
            Pages: pages,
            PublishDate: "1965",
            LanguageIso639: languageIso639,
            Binding: binding,
            EditionStatement: "1st",
            ImageThumbnail: null,
            ImageUrl: null,
            SaxoUrl: null,
            Msrp: 199.95m,
            Dimensions: null,
            DataSource: DataSource.IsbndbApi));
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
            Publisher: null,
            Pages: null,
            PublishDate: null,
            LanguageIso639: "en",
            Binding: null,
            EditionStatement: null,
            ImageThumbnail: null,
            ImageUrl: null,
            SaxoUrl: null,
            Msrp: null,
            Dimensions: null,
            DataSource: DataSource.IsbndbApi));

        Assert.Equal(workId, edition.WorkId);
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
    public void Create_NormalizesBinding()
    {
        var edition = CreateValidEdition(binding: "Hardcover");

        Assert.Equal(BookFormat.Hardback, edition.Binding);
    }

    [Fact]
    public void UpdateDataSource_ChangesDataSource()
    {
        var edition = CreateValidEdition();

        edition.UpdateDataSource(DataSource.GoogleBooksApi);

        Assert.Equal(DataSource.GoogleBooksApi, edition.DataSource);
    }

    [Fact]
    public void SetPublisher_AssignsPublisher()
    {
        var edition = CreateValidEdition();
        var publisher = Publisher.TryCreate("Ace Books");

        edition.SetPublisher(publisher);

        Assert.Equal(publisher, edition.Publisher);
    }

    [Fact]
    public void Reconstitute_PreservesData()
    {
        var id = Guid.NewGuid();
        var workId = Guid.NewGuid();
        var data = new EditionReconstitutionData(
            Id: id,
            WorkId: workId,
            Isbn13: "9781402894626",
            Isbn10: "1402894627",
            Pages: 412,
            PublicationDate: "1965",
            Language: "en",
            EditionStatement: "1st",
            Binding: BookFormat.Hardback,
            ImageThumbnailUrl: null,
            CoverImageUrl: null,
            SaxoUrl: null,
            Msrp: 199.95m,
            Dimensions: null,
            DataSource: DataSource.IsbndbApi,
            Publisher: Publisher.TryCreate("Chilton Books"));

        var edition = Edition.Reconstitute(data);

        Assert.Equal(id, edition.Id.Value);
        Assert.Equal(workId, edition.WorkId.Value);
        Assert.Equal("9781402894626", edition.Isbn?.Value13);
        Assert.Equal(412, edition.Pages);
        Assert.Equal("en", edition.Language?.Value);
        Assert.Equal("Chilton Books", edition.Publisher?.Name);
    }

    [Fact]
    public void Create_WithBothIsbnsNull_Throws()
    {
        Assert.Throws<MissingIsbnException>(() => CreateValidEdition(isbn13: null, isbn10: null));
    }
}