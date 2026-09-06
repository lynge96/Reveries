using Reveries.Domain.Editions;
using Reveries.Domain.Enums;
using Reveries.Domain.Publishers;
using Reveries.Persistence.Records;

namespace Reveries.Persistence.Mappers;

public static class EditionMappingExtensions
{
    public static EditionRecord ToRecord(this Edition edition)
    {
        return new EditionRecord
        {
            Id = edition.Id.Value,
            WorkId = edition.WorkId.Value,
            Isbn13 = edition.Isbn?.Value13,
            Isbn10 = edition.Isbn?.Value10,
            PageCount = edition.Pages,
            PublicationDate = edition.PublicationDate?.Value,
            Language = edition.Language?.Value,
            EditionStatement = edition.EditionDescription,
            ImageUrl = edition.Cover?.Url,
            ImageThumbnail = edition.Cover?.ThumbnailUrl,
            SaxoUrl = edition.SaxoUrl?.Value,
            HeightCm = edition.Dimensions?.HeightCm,
            WidthCm = edition.Dimensions?.WidthCm,
            ThicknessCm = edition.Dimensions?.ThicknessCm,
            WeightG = edition.Dimensions?.WeightG,
            Format = edition.Format.ToString(),
            PublisherId = edition.PublisherId?.Value
        };
    }

    public static Edition ToDomain(this EditionRecord record)
    {
        var data = new EditionReconstitutionData
        (
            Id: record.Id,
            WorkId: record.WorkId,
            Isbn13: record.Isbn13,
            Isbn10: record.Isbn10,
            Pages: record.PageCount,
            PublicationDate: record.PublicationDate,
            Language: record.Language,
            EditionStatement: record.EditionStatement,
            Format: ParseFormat(record.Format),
            ImageThumbnailUrl: record.ImageThumbnail,
            CoverImageUrl: record.ImageUrl,
            SaxoUrl: record.SaxoUrl,
            Dimensions: BookDimensions.Reconstitute(record.HeightCm, record.WidthCm, record.ThicknessCm, record.WeightG),
            PublisherId: record.PublisherId is { } publisherId
                ? new PublisherId(publisherId)
                : null
        );

        return Edition.Reconstitute(data);
    }

    private static BookFormat ParseFormat(string? value)
    {
        return Enum.TryParse<BookFormat>(value, out var format)
            ? format
            : BookFormat.Unknown;
    }
}