using Reveries.Domain.Enums;
using Reveries.Domain.Exceptions;
using Reveries.Domain.Helpers;
using Reveries.Domain.Publishers;
using Reveries.Domain.Works;
namespace Reveries.Domain.Editions;

public class Edition
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
    public BookFormat Binding { get; private init; }
    public string? ImageThumbnailUrl { get; private init; }
    public string? CoverImageUrl { get; private init; }
    public SaxoUrl? SaxoUrl { get; private init; }
    public decimal? Msrp { get; private init; }
    public BookDimensions? Dimensions { get; private init; }
    public DataSource DataSource { get; private set; }

    private Edition() { }

    public static Edition Create(EditionData data)
    {
        if (data.Isbn13 == null && data.Isbn10 == null)
            throw new MissingIsbnException();

        var edition = new Edition
        {
            Id = EditionId.New(),
            WorkId = data.WorkId,
            Isbn13 = data.Isbn13 != null ? Isbn.Create(data.Isbn13) : null,
            Isbn10 = data.Isbn10 != null ? Isbn.Create(data.Isbn10) : null,
            Publisher = Publisher.TryCreate(data.Publisher),
            Language = data.LanguageIso639.GetLanguageName(),
            PublicationDate = data.PublishDate,
            EditionStatement = data.EditionStatement,
            Binding = data.Binding.GetStandardBinding(),
            ImageThumbnailUrl = data.ImageThumbnail,
            CoverImageUrl = data.ImageUrl,
            SaxoUrl = SaxoUrl.TryCreate(data.SaxoUrl),
            Msrp = data.Msrp,
            Dimensions = data.Dimensions,
            DataSource = data.DataSource
        };

        edition.SetPages(data.Pages);

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
            SaxoUrl = data.SaxoUrl != null ? new SaxoUrl(data.SaxoUrl) : null,
            Msrp = data.Msrp,
            Dimensions = data.Dimensions,
            Publisher = data.Publisher,
            DataSource = data.DataSource
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
            case <= 0:
                throw new InvalidPageCountException(pages);
            default:
                Pages = pages;
                break;
        }
    }
}