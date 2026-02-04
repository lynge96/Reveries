using Reveries.Core.Models;
using Reveries.Core.ValueObjects;
using Reveries.Infrastructure.Postgresql.Entities;

namespace Reveries.Infrastructure.Postgresql.Mappers;

public static class DomainToEntityExtensions
{
    public static BookEntity ToDbModel(this Book model)
    {
        return new BookEntity
        {
            BookDomainId = model.Id.Value,
            Title = model.Title,
            Isbn13 = model.Isbn13?.Value,
            Isbn10 = model.Isbn10?.Value,
            PageCount = model.Pages,
            IsRead = model.IsRead,
            PublicationDate = model.PublishDate,
            Synopsis = model.Synopsis,
            Language = model.Language,
            Edition = model.Edition,
            Binding = model.Binding,
            CoverImageUrl = model.CoverImageUrl,
            ImageThumbnailUrl = model.ImageThumbnailUrl,
            Msrp = model.Msrp,
            SeriesNumber = model.SeriesNumber,
            HeightCm = model.Dimensions?.HeightCm,
            WidthCm = model.Dimensions?.WidthCm,
            ThicknessCm = model.Dimensions?.ThicknessCm,
            WeightG = model.Dimensions?.WeightG,
            DateCreatedBook = model.DateCreated
        };
    }
    
    public static PublisherEntity ToDbModel(this Publisher model)
    {
        return new PublisherEntity
        {
            PublisherName = model.Name ?? string.Empty,
            DateCreatedPublisher = model.DateCreated
        };
    }

    public static SeriesEntity ToDbModel(this Series model)
    {
        return new SeriesEntity
        {
            SeriesName = model.Name ?? string.Empty,
            DateCreatedSeries = model.DateCreated
        };
    }

    public static AuthorEntity ToDbModel(this Author model)
    {
        return new AuthorEntity
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            NormalizedName = model.NormalizedName,
            DateCreatedAuthor = model.DateCreated,
            AuthorNameVariants = model.NameVariants
                .Select(nv => nv.ToDbModel())
                .ToList()
        };
    }

    public static AuthorNameVariantEntity ToDbModel(this AuthorNameVariant variant)
    {
        return new AuthorNameVariantEntity
        {
            IsPrimary = variant.IsPrimary,
            NameVariant = variant.NameVariant
        };
    }
    
    public static GenreEntity ToDbModel(this Genre model)
    {
        return new GenreEntity
        {
            Name = model.Value
        };
    }

    public static DeweyDecimalEntity ToDbModel(this DeweyDecimal model)
    {
        return new DeweyDecimalEntity
        {
            Code = model.Code
        };
    }
}