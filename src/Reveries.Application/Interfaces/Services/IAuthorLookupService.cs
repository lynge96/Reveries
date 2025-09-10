using Reveries.Core.Entities;

namespace Reveries.Application.Interfaces.Services;

public interface IAuthorLookupService
{
    Task<List<Author>> FindAuthorsByNameAsync(string name, CancellationToken cancellationToken = default);
}