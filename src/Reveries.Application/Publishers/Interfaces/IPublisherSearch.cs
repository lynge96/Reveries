using Reveries.Application.Books.Models;
using Reveries.Domain.Publishers;

namespace Reveries.Application.Publishers.Interfaces;

public interface IPublisherSearch
{
    Task<List<EditionWithWork>?> GetBooksByPublisherAsync(Publisher publisher, CancellationToken ct = default);
    Task<List<Publisher>?> GetPublishersByNameAsync(Publisher publisher, CancellationToken ct = default);
}
