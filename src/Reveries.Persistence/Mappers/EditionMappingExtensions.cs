using Reveries.Domain.Editions;
using Reveries.Domain.Enums;
using Reveries.Domain.Publishers;
using Reveries.Domain.Shared;
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
            Isbn13 = edition.Isbn13?.ToString(),
            Isbn10 = edition.Isbn10?.ToString(),
            PageCount = edition.Pages,
            PublicationDate = edition.PublicationDate,
            Language = edition.Language,
            EditionStatement = edition.EditionStatement,
            Binding = edition.Binding,
            ImageUrl = edition.CoverImageUrl,
            ImageThumbnail = edition.ImageThumbnailUrl,
            Msrp = edition.Msrp,
            HeightCm = edition.Dimensions?.HeightCm,
            WidthCm = edition.Dimensions?.WidthCm,
            ThicknessCm = edition.Dimensions?.ThicknessCm,
            WeightG = edition.Dimensions?.WeightG,
            DataSource = edition.DataSource.ToString(),
            PublisherId = edition.Publisher?.Id.Value,
            DateCreated = edition.DateCreated
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
            Binding: view.Binding,
            ImageThumbnailUrl: view.ImageThumbnailUrl,
            CoverImageUrl: view.CoverImageUrl,
            Msrp: view.Msrp,
            Dimensions: BookDimensions.Create(view.HeightCm, view.WidthCm, view.ThicknessCm, view.WeightG),
            DataSource: ParseDataSource(view.DataSource),
            Publisher: view.PublisherId is { } publisherId
                ? Publisher.Reconstitute(new PublisherId(publisherId), view.PublisherName!, view.DateCreatedPublisher)
                : null,
            DateCreated: view.DateCreatedEdition
        );

        return Edition.Reconstitute(data);
    }

    private static DataSource ParseDataSource(string? value)
    {
        return Enum.TryParse<DataSource>(value, out var dataSource)
            ? dataSource
            : DataSource.Database;
    }
}