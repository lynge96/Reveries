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
            Name = publisher.Name
        };
    }

    public static SeriesEntity ToEntity(this Series series)
    {
        return new SeriesEntity
        {
            Id = series.Id.Value,
            Name = series.Name
        };
    }

    public static AuthorEntity ToEntity(this Author author)
    {
        return new AuthorEntity
        {
            Id = author.Id.Value,
            Name = author.Name,
            NormalizedName = author.NormalizedName
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
