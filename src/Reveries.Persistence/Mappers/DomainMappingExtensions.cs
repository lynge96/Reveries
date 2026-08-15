using Reveries.Domain.Enums;
using Reveries.Domain.Identity;
using Reveries.Domain.Models;
using Reveries.Domain.ValueObjects;
using Reveries.Persistence.Entities;

namespace Reveries.Persistence.Mappers;

public static class DomainMappingExtensions
{
    public static Book ToDomain(this BookEntity bookEntity)
    {
        var bookData = new BookReconstitutionData
        (
            Id: bookEntity.Id,
            Title: bookEntity.Title,
            Isbn13: bookEntity.Isbn13,
            Isbn10: bookEntity.Isbn10,
            Pages: bookEntity.PageCount,
            PublicationDate: bookEntity.PublicationDate,
            Language: bookEntity.Language,
            Synopsis: bookEntity.Synopsis,
            ImageThumbnailUrl: bookEntity.ImageThumbnail,
            CoverImageUrl: bookEntity.ImageUrl,
            Msrp: bookEntity.Msrp,
            Binding: bookEntity.Binding,
            Edition: bookEntity.Edition,
            SeriesNumber: bookEntity.SeriesNumber,
            Dimensions: BookDimensions.Create(bookEntity.HeightCm, bookEntity.WidthCm, bookEntity.ThicknessCm, bookEntity.WeightG),
            DataSource: DataSource.Database,
            DateCreated: bookEntity.DateCreated
        );
        
        return Book.Reconstitute(bookData);
    }
    
    public static Publisher ToDomain(this PublisherEntity publisherEntity)
    {
        return Publisher.Reconstitute(
            new PublisherId(publisherEntity.Id),
            publisherEntity.Name,
            publisherEntity.DateCreated
        );
    }

    public static Series ToDomain(this SeriesEntity seriesEntity)
    {
        return Series.Reconstitute(
            new SeriesId(seriesEntity.Id), 
            seriesEntity.Name, 
            seriesEntity.DateCreated
        );
    }

    public static Author ToDomain(this AuthorEntity authorEntity)
    {
        return Author.Reconstitute(
            new AuthorId(authorEntity.Id), 
            authorEntity.FirstName, 
            authorEntity.LastName, 
            authorEntity.DateCreated
        );
    }

    public static Genre ToDomain(this GenreEntity genreEntity)
    {
        return Genre.Create(genreEntity.Name);
    }

    public static DeweyDecimal ToDomain(this DeweyDecimalEntity deweyDecimalEntity)
    {
        return DeweyDecimal.Create(deweyDecimalEntity.Code);
    }
}