using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Mappers;

public static class EntityMappingExtensions
{
    public static BookEntity ToEntity(this Book book)
    {
        return new BookEntity
        {
            Id = book.Id.Value,
            Title = book.Title,
            Isbn13 = book.Isbn13?.Value,
            Isbn10 = book.Isbn10?.Value,
            PageCount = book.Pages,
            IsRead = book.IsRead,
            PublicationDate = book.PublicationDate,
            Synopsis = book.Synopsis,
            Language = book.Language,
            Edition = book.Edition,
            Binding = book.Binding,
            CoverImageUrl = book.CoverImageUrl,
            ImageThumbnailUrl = book.ImageThumbnailUrl,
            Msrp = book.Msrp,
            SeriesNumber = book.SeriesNumber,
            HeightCm = book.Dimensions?.HeightCm,
            WidthCm = book.Dimensions?.WidthCm,
            ThicknessCm = book.Dimensions?.ThicknessCm,
            WeightG = book.Dimensions?.WeightG,
            DateCreated = book.DateCreated,
            
            PublisherId = book.Publisher?.Id.Value,
            SeriesId = book.Series?.Id.Value,
        };
    }
    
    public static PublisherEntity ToEntity(this Publisher publisher)
    {
        return new PublisherEntity
        {
            Id = publisher.Id.Value,
            Name = publisher.Name,
            DateCreated = publisher.DateCreated
        };
    }

    public static SeriesEntity ToEntity(this Series series)
    {
        return new SeriesEntity
        {
            Id = series.Id.Value,
            Name = series.Name,
            DateCreated = series.DateCreated
        };
    }

    public static AuthorEntity ToEntity(this Author author)
    {
        return new AuthorEntity
        {
            Id = author.Id.Value,
            FirstName = author.FirstName,
            LastName = author.LastName,
            NormalizedName = author.NormalizedName,
            DateCreated = author.DateCreated,
            AuthorNameVariants = author.NameVariants
                .Select(v => v.ToEntity(author.Id.Value))
                .ToList()
        };
    }

    private static AuthorNameVariantEntity ToEntity(this AuthorNameVariant variant, Guid authorId)
    {
        return new AuthorNameVariantEntity
        {
            AuthorId = authorId,
            IsPrimary = variant.IsPrimary,
            NameVariant = variant.NameVariant
        };
    }
    
    public static GenreEntity ToEntity(this Genre genre)
    {
        return new GenreEntity
        {
            Name = genre.Value
        };
    }

    public static DeweyDecimalEntity ToEntity(this DeweyDecimal deweyDecimal)
    {
        return new DeweyDecimalEntity
        {
            Code = deweyDecimal.Code
        };
    }
}