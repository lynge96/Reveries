using Reveries.Core.Models;

namespace Reveries.Application.Authors.Interfaces;

public interface IAuthorLookupService
{
    Task<List<Author>> FindAuthorsByNameAsync(Author author, CancellationToken ct = default);
}