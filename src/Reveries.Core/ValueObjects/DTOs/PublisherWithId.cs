using Reveries.Core.Models;

namespace Reveries.Core.ValueObjects.DTOs;

public record PublisherWithId
{
    public Publisher Publisher { get; init; }
    public int DbId { get; init; }

    public PublisherWithId(Publisher publisher, int dbId)
    {
        Publisher = publisher;
        DbId = dbId;
    }
}
