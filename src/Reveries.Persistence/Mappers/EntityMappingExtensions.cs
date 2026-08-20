using Reveries.Domain.Authors;
using Reveries.Domain.Publishers;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Works;
using Reveries.Persistence.Entities;

namespace Reveries.Persistence.Mappers;

public static class EntityMappingExtensions
{
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
            Name = genre.Name
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
