using Reveries.Domain.Authors;
using Reveries.Domain.Publishers;
using Reveries.Domain.BookSeries;
using Reveries.Persistence.Records;

namespace Reveries.Persistence.Mappers;

public static class DomainMappingExtensions
{
    public static Publisher ToDomain(this PublisherRecord record)
    {
        return Publisher.Reconstitute(
            new PublisherId(record.Id),
            record.Name
        );
    }

    public static Series ToDomain(this SeriesRecord record)
    {
        return Series.Reconstitute(
            new SeriesId(record.Id),
            record.Name
        );
    }

    public static Author ToDomain(this AuthorRecord record)
    {
        return Author.Reconstitute(
            new AuthorId(record.Id),
            record.Name
        );
    }
}