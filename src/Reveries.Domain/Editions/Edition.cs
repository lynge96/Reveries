using Reveries.Domain.Enums;
using Reveries.Domain.Exceptions;
using Reveries.Domain.Helpers;
using Reveries.Domain.Publishers;
using Reveries.Domain.Shared;
using Reveries.Domain.Works;
namespace Reveries.Domain.Editions;

public class Edition : BaseEntity
{
    public EditionId Id { get; private init; }
    public WorkId WorkId { get; private init; }
    public Isbn? Isbn13 { get; private init; }
    public Isbn? Isbn10 { get; private init; }
    public int? Pages { get; private set; }
    public Publisher? Publisher { get; private set; }
    public string? Language { get; private init; }
    public string? PublicationDate { get; private init; }
    public string? EditionStatement { get; private init; }
    public string? Binding { get; private init; }
    public string? ImageThumbnailUrl { get; private init; }
    public string? CoverImageUrl { get; private init; }
    public decimal? Msrp { get; private init; }
    public BookDimensions? Dimensions { get; private init; }
    public DataSource DataSource { get; private set; }

    private Edition() { }

    public static Edition Create(
        WorkId workId,
        string? isbn13,
        string? isbn10,
        string? publisher,
        int? pages,
        string? publishDate,
        string? languageIso639,
        string? binding,
        string? editionStatement,
        string? imageThumbnail,
        string? imageUrl,
        decimal? msrp,
        decimal? height,
        decimal? width,
        decimal? thickness,
        decimal? weight,
        DataSource dataSource)
    {
        if (isbn13 == null && isbn10 == null)
            throw new MissingIsbnException();

        var edition = new Edition
        {
            Id = EditionId.New(),
            WorkId = workId,
            Isbn13 = isbn13 != null ? Isbn.Create(isbn13) : null,
            Isbn10 = isbn10 != null ? Isbn.Create(isbn10) : null,
            Publisher = publisher != null ? Publisher.Create(publisher) : null,
            Language = languageIso639.GetLanguageName(),
            PublicationDate = publishDate,
            EditionStatement = editionStatement,
            Binding = binding?.GetStandardBinding(),
            ImageThumbnailUrl = imageThumbnail,
            CoverImageUrl = imageUrl,
            Msrp = msrp,
            Dimensions = BookDimensions.Create(height, width, thickness, weight),
            DataSource = dataSource
        };

        edition.SetPages(pages);

        return edition;
    }

    public static Edition Reconstitute(EditionReconstitutionData data)
    {
        return new Edition
        {
            Id = new EditionId(data.Id),
            WorkId = new WorkId(data.WorkId),
            Isbn13 = data.Isbn13 != null ? new Isbn(data.Isbn13) : null,
            Isbn10 = data.Isbn10 != null ? new Isbn(data.Isbn10) : null,
            Pages = data.Pages,
            PublicationDate = data.PublicationDate,
            Language = data.Language,
            EditionStatement = data.EditionStatement,
            Binding = data.Binding,
            ImageThumbnailUrl = data.ImageThumbnailUrl,
            CoverImageUrl = data.CoverImageUrl,
            Msrp = data.Msrp,
            Dimensions = data.Dimensions,
            Publisher = data.Publisher,
            DataSource = data.DataSource,
            DateCreated = data.DateCreated
        };
    }

    public void SetPublisher(Publisher? publisher) => Publisher = publisher;

    public void UpdateDataSource(DataSource newDataSource)
    {
        if (DataSource == newDataSource) return;
        DataSource = newDataSource;
    }

    private void SetPages(int? pages)
    {
        switch (pages)
        {
            case null:
                return;
            case < 0:
                throw new InvalidPageCountException(pages);
            default:
                Pages = pages;
                break;
        }
    }
}