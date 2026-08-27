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
    public Isbn? Isbn { get; private init; }
    public int? Pages { get; private init; }
    public Publisher? Publisher { get; private set; }
    public Language? Language { get; private init; }
    public PublicationDate? PublicationDate { get; private init; }
    public string? EditionDescription { get; private init; }
    public BookFormat Format { get; private init; }
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
            Isbn = data.Isbn13 != null ? Isbn.Create(data.Isbn13) : Isbn.Create(data.Isbn10!),
            Pages = data.Pages > 0 ? data.Pages : null,
            Publisher = Publisher.TryCreate(data.Publisher),
            Language = Language.TryCreate(data.LanguageIso639),
            PublicationDate = PublicationDate.TryCreate(data.PublishDate),
            EditionDescription = EditionDescriptionNormalizer.Normalize(data.EditionStatement),
            Format = data.Format.GetStandardFormat(),
            ImageThumbnailUrl = data.ImageThumbnail,
            CoverImageUrl = data.ImageUrl,
            SaxoUrl = SaxoUrl.TryCreate(data.SaxoUrl),
            Msrp = data.Msrp,
            Dimensions = data.Dimensions,
            DataSource = data.DataSource
        };

        return edition;
    }

    public static Edition Reconstitute(EditionReconstitutionData data)
    {
        return new Edition
        {
            Id = new EditionId(data.Id),
            WorkId = new WorkId(data.WorkId),
            Isbn = BuildReconstitutedIsbn(data.Isbn13, data.Isbn10),
            Pages = data.Pages,
            PublicationDate = PublicationDate.TryCreate(data.PublicationDate),
            Language = data.Language != null ? Language.Reconstitute(data.Language) : null,
            EditionDescription = data.EditionStatement,
            Format = data.Format,
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

    private static Isbn? BuildReconstitutedIsbn(string? value13, string? value10)
    {
        if (value13 != null)
            return Isbn.Reconstitute(value13, value10);

        return value10 != null ? Isbn.Create(value10) : null;
    }
}