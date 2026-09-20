using Reveries.Domain.Authors;
using Reveries.Domain.Publishers;
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

    public static Author ToDomain(this AuthorRecord record)
    {
        return Author.Reconstitute(
            new AuthorId(record.Id),
            record.Name
        );
    }
}