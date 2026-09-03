using Reveries.Domain.Editions;
using Reveries.Domain.Enums;
using Reveries.Domain.Publishers;
using Reveries.Persistence.Entities;
using Reveries.Persistence.Views;

namespace Reveries.Persistence.Mappers;

public static class EditionMappingExtensions
{
    public static EditionEntity ToEntity(this Edition edition)
    {
        return new EditionEntity
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

    public static Edition ToDomain(this EditionsView view)
    {
        var data = new EditionReconstitutionData
        (
            Id: view.Id,
            WorkId: view.WorkId,
            Isbn13: view.Isbn13,
            Isbn10: view.Isbn10,
            Pages: view.PageCount,
            PublicationDate: view.PublicationDate,
            Language: view.Language,
            EditionStatement: view.EditionStatement,
            Format: ParseFormat(view.Format),
            ImageThumbnailUrl: view.ImageThumbnailUrl,
            CoverImageUrl: view.CoverImageUrl,
            SaxoUrl: view.SaxoUrl,
            Dimensions: BookDimensions.Reconstitute(view.HeightCm, view.WidthCm, view.ThicknessCm, view.WeightG),
            PublisherId: view.PublisherId is { } publisherId
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