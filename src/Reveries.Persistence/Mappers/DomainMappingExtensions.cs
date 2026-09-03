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
            publisherEntity.Name
        );
    }

    public static Series ToDomain(this SeriesEntity seriesEntity)
    {
        return Series.Reconstitute(
            new SeriesId(seriesEntity.Id),
            seriesEntity.Name
        );
    }

    public static Author ToDomain(this AuthorEntity authorEntity)
    {
        return Author.Reconstitute(
            new AuthorId(authorEntity.Id),
            authorEntity.Name
        );
    }
}
