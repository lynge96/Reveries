using Reveries.Application.Authors.Interfaces;
using Reveries.Domain.Authors;
using Reveries.Domain.Interfaces.Repositories;

namespace Reveries.Application.Authors.Services;

public class AuthorResolver : IAuthorResolver
{
    private readonly IAuthorRepository _authors;

    public AuthorResolver(IAuthorRepository authors)
    {
        _authors = authors;
    }

    public async Task<List<AuthorId>> ResolveIdsAsync(IReadOnlyList<Author> authors, CancellationToken ct = default)
    {
        if (authors.Count == 0)
            return [];

        var names = authors.Select(a => a.Name).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

        var existing = await _authors.GetByNamesAsync(names, ct);
        var byName = existing.ToDictionary(a => a.Name, a => a, StringComparer.OrdinalIgnoreCase);

        var missing = authors
            .Where(a => !byName.ContainsKey(a.Name))
            .DistinctBy(a => a.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (missing.Count > 0)
        {
            await _authors.AddRangeAsync(missing, ct);
            foreach (var author in missing)
                byName[author.Name] = author;
        }

        return authors
            .Select(a => byName[a.Name].Id)
            .Distinct()
            .ToList();
    }
}