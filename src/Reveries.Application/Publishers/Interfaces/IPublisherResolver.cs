using Reveries.Domain.Publishers;

namespace Reveries.Application.Publishers.Interfaces;

public interface IPublisherResolver
{
    Task<Publisher?> ResolveAsync(Publisher? publisher, CancellationToken ct = default);
}