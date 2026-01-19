using Reveries.Core.Enums;
using Reveries.Core.Models;
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
            dataSource: DataSource.Database
            );
    }
    
    public static Publisher ToDomain(this PublisherEntity entity)
    {
        return new Publisher
        {
            Id = entity.PublisherId,
            Name = entity.Name,
            DateCreated = entity.DateCreatedPublisher
        };
    }

    public static Series ToDomain(this SeriesEntity entity)
    {
        return new Series
        {
            Id = entity.SeriesId,
            Name = entity.SeriesName,
            DateCreated = entity.DateCreatedSeries
        };
    }

    public static BookDimensions ToDomain(this DimensionsEntity entity)
    {
        return new BookDimensions
        {
            HeightCm = entity.HeightCm,
            WidthCm = entity.WidthCm,
            ThicknessCm = entity.ThicknessCm,
            WeightG = entity.WeightG,
        };
    }

    public static Author ToDomain(this AuthorEntity entity)
    {
        return new Author
        {
            Id = entity.AuthorId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            NormalizedName = entity.NormalizedName!,
            DateCreated = entity.DateCreatedAuthor
        };
    }

    public static Subject ToDomain(this SubjectEntity entity)
    {
        return new Subject
        {
            Id = entity.SubjectId,
            Genre = entity.Genre!,
            DateCreated = entity.DateCreatedSubject
        };
    }

    public static DeweyDecimal ToDomain(this DeweyDecimalEntity entity)
    {
        return new DeweyDecimal
        {
            Code = entity.Code
        };
    }
}