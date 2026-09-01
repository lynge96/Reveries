using Reveries.Domain.Common;
using Reveries.Domain.Enums;
using Reveries.Domain.Exceptions;
using Reveries.Domain.Helpers;
using Reveries.Domain.Publishers;
using Reveries.Domain.Works;
namespace Reveries.Domain.Editions;

public class Edition : Entity<EditionId>
{
    public WorkId WorkId { get; private init; }
    public Isbn? Isbn { get; private init; }
    public int? Pages { get; private init; }
    public Publisher? Publisher { get; private set; }
    public Language? Language { get; private init; }
    public PublicationDate? PublicationDate { get; private init; }
    public string? EditionDescription { get; private init; }
    public BookFormat Format { get; private init; }
    public Cover? Cover { get; private set; }
    public SaxoUrl? SaxoUrl { get; private init; }
    public BookDimensions? Dimensions { get; private init; }

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
            Pages = PageCountNormalizer.Normalize(data.Pages),
            Publisher = Publisher.TryCreate(data.Publisher),
            Language = Language.TryCreate(data.LanguageIso639),
            PublicationDate = PublicationDate.TryCreate(data.PublishDate),
            EditionDescription = EditionDescriptionNormalizer.Normalize(data.EditionStatement),
            Format = data.Format.GetStandardFormat(),
            Cover = Cover.TryCreate(url: data.ImageUrl, thumbnailUrl: data.ImageThumbnail),
            SaxoUrl = SaxoUrl.TryCreate(data.SaxoUrl),
            Dimensions = data.Dimensions
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
            PublicationDate = PublicationDate.Reconstitute(data.PublicationDate),
            Language = data.Language != null ? Language.Reconstitute(data.Language) : null,
            EditionDescription = data.EditionStatement,
            Format = data.Format,
            Cover = Cover.Reconstitute(url: data.CoverImageUrl, thumbnailUrl: data.ImageThumbnailUrl),
            SaxoUrl = data.SaxoUrl != null ? new SaxoUrl(data.SaxoUrl) : null,
            Dimensions = data.Dimensions,
            Publisher = data.Publisher
        };
    }

    public void SetPublisher(Publisher? publisher) => Publisher = publisher;

    public void SetCover(Cover? cover) => Cover = cover;

    private static Isbn? BuildReconstitutedIsbn(string? value13, string? value10)
    {
        if (value13 != null)
            return Isbn.Reconstitute(value13, value10);

        return value10 != null ? Isbn.Create(value10) : null;
    }
}