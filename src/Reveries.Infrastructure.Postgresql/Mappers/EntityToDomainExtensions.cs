using Reveries.Core.Enums;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Mappers;

public static class EntityToDomainExtensions
{
    public static Book ToDomain(this BookEntity entity)
    {
        return Book.Reconstitute(
            id: entity.Id,
            isbn13: entity.Isbn13,
            isbn10: entity.Isbn10,
            title: entity.Title,
            pages: entity.Pages,
            isRead: entity.IsRead,
            publishDate: entity.PublishDate,
            language: entity.Language,
            synopsis: entity.Synopsis,
            imageThumbnail: entity.ImageThumbnail,
            imageUrl: entity.ImageUrl,
            msrp: entity.Msrp,
            binding: entity.Binding,
            edition: entity.Edition,
            seriesNumber: entity.SeriesNumber,
            dataSource: DataSource.Database,
            dateCreated: entity.DateCreated
            );
    }
    
    public static Publisher ToDomain(this PublisherEntity entity)
    {
        return Publisher.Reconstitute(entity.PublisherId, entity.Name, entity.DateCreatedPublisher);
    }

    public static Series ToDomain(this SeriesEntity entity)
    {
        return Series.Reconstitute(entity.SeriesId, entity.SeriesName, entity.DateCreatedSeries);
    }

    public static BookDimensions? ToDomain(this DimensionsEntity entity)
    {
        return BookDimensions.Create(entity.HeightCm, entity.WidthCm, entity.ThicknessCm, entity.WeightG);
    }

    public static Author ToDomain(this AuthorEntity entity)
    {
        return Author.Reconstitute(entity.AuthorId, entity.NormalizedName, entity.FirstName, entity.LastName, entity.DateCreatedAuthor);
    }

    public static Subject ToDomain(this SubjectEntity entity)
    {
        return Subject.Reconstitute(entity.SubjectId, entity.Genre!, entity.DateCreatedSubject);
    }

    public static DeweyDecimal ToDomain(this DeweyDecimalEntity entity)
    {
        return DeweyDecimal.Create(entity.Code!);
    }
}