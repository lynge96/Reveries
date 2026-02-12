using Reveries.Core.Enums;
using Reveries.Core.Identity;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Mappers;

public static class EntityToDomainExtensions
{
    public static Book ToDomain(this BookEntity entity)
    {
        var bookData = new BookReconstitutionData
        (
            Id: entity.BookDomainId,
            Title: entity.Title,
            Isbn13: entity.Isbn13,
            Isbn10: entity.Isbn10,
            Pages: entity.PageCount,
            IsRead: entity.IsRead,
            PublicationDate: entity.PublicationDate,
            Language: entity.Language,
            Synopsis: entity.Synopsis,
            ImageThumbnailUrl: entity.ImageThumbnailUrl,
            CoverImageUrl: entity.CoverImageUrl,
            Msrp: entity.Msrp,
            Binding: entity.Binding,
            Edition: entity.Edition,
            SeriesNumber: entity.SeriesNumber,
            Dimensions: BookDimensions.Create(entity.HeightCm, entity.WidthCm, entity.ThicknessCm, entity.WeightG),
            DataSource: DataSource.Database,
            DateCreated: entity.DateCreatedBook
        );
        
        return Book.Reconstitute(bookData);
    }
    
    public static Publisher ToDomain(this PublisherEntity entity)
    {
        return Publisher.Reconstitute(new PublisherId(entity.PublisherDomainId), entity.PublisherName, entity.DateCreatedPublisher);
    }

    public static Series ToDomain(this SeriesEntity entity)
    {
        return Series.Reconstitute(new SeriesId(entity.SeriesDomainId), entity.SeriesName, entity.DateCreatedSeries);
    }

    public static Author ToDomain(this AuthorEntity entity)
    {
        return Author.Reconstitute(new AuthorId(entity.AuthorDomainId), entity.FirstName, entity.LastName, entity.DateCreatedAuthor);
    }

    public static Genre ToDomain(this GenreEntity entity)
    {
        return Genre.Create(entity.Name);
    }

    public static DeweyDecimal ToDomain(this DeweyDecimalEntity entity)
    {
        return DeweyDecimal.Create(entity.Code);
    }
}