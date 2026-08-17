using Reveries.Domain.Authors;
using Reveries.Domain.Publishers;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Works;
using Reveries.Persistence.Entities;

namespace Reveries.Persistence.Mappers;

public static class DomainMappingExtensions
{
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
