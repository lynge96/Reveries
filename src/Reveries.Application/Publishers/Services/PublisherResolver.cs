using Reveries.Application.Publishers.Interfaces;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Publishers;

namespace Reveries.Application.Publishers.Services;

public class PublisherResolver : IPublisherResolver
{
    private readonly IPublisherRepository _publishers;

    public PublisherResolver(IPublisherRepository publishers)
    {
        _publishers = publishers;
    }

    public async Task<Publisher?> ResolveAsync(Publisher? publisher, CancellationToken ct = default)
    {
        if (publisher is null)
            return null;

        var existing = await _publishers.GetByNameAsync(publisher.Name, ct);
        if (existing is not null)
            return existing;

        await _publishers.AddAsync(publisher, ct);

        return publisher;
    }
}