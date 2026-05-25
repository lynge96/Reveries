using Reveries.Core.Models;

namespace Reveries.Application.Publishers.Interfaces;

public interface IPublisherSearch
{
    Task<List<Book>?> GetBooksByPublisherAsync(Publisher publisher, CancellationToken ct = default);
    Task<List<Publisher>?> GetPublishersByNameAsync(Publisher publisher, CancellationToken ct = default);
}