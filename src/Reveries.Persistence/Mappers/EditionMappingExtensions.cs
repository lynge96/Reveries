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
            PublicationDate = edition.PublicationDate,
            Language = edition.Language?.Value,
            EditionStatement = edition.EditionStatement,
            ImageUrl = edition.CoverImageUrl,
            ImageThumbnail = edition.ImageThumbnailUrl,
            SaxoUrl = edition.SaxoUrl?.Value,
            Msrp = edition.Msrp,
            HeightCm = edition.Dimensions?.HeightCm,
            WidthCm = edition.Dimensions?.WidthCm,
            ThicknessCm = edition.Dimensions?.ThicknessCm,
            WeightG = edition.Dimensions?.WeightG,
            DataSource = edition.DataSource.ToString(),
            Binding = edition.Binding.ToString(),
            PublisherId = edition.Publisher?.Id.Value
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
            Binding: ParseBinding(view.Binding),
            ImageThumbnailUrl: view.ImageThumbnailUrl,
            CoverImageUrl: view.CoverImageUrl,
            SaxoUrl: view.SaxoUrl,
            Msrp: view.Msrp,
            Dimensions: BookDimensions.Create(view.HeightCm, view.WidthCm, view.ThicknessCm, view.WeightG),
            DataSource: ParseDataSource(view.DataSource),
            Publisher: view.PublisherId is { } publisherId
                ? Publisher.Reconstitute(new PublisherId(publisherId), view.PublisherName!)
                : null
        );

        return Edition.Reconstitute(data);
    }

    private static DataSource ParseDataSource(string? value)
    {
        return Enum.TryParse<DataSource>(value, out var dataSource)
            ? dataSource
            : DataSource.Database;
    }

    private static BookFormat ParseBinding(string? value)
    {
        return Enum.TryParse<BookFormat>(value, out var binding)
            ? binding
            : BookFormat.Unknown;
    }
}