using Reveries.Application.Books.Interfaces;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Services;

public class GenreResolver : IGenreResolver
{
    private readonly IGenreRepository _genres;

    public GenreResolver(IGenreRepository genres)
    {
        _genres = genres;
    }

    public async Task<Dictionary<string, int>> ResolveIdsAsync(IReadOnlyList<Genre> genres, CancellationToken ct = default)
    {
        if (genres.Count == 0)
            return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        var names = genres.Select(g => g.Name).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

        var byName = await _genres.GetByNamesAsync(names, ct);

        var missing = names.Where(name => !byName.ContainsKey(name)).ToArray();
        if (missing.Length > 0)
        {
            var created = await _genres.AddRangeAsync(missing, ct);
            foreach (var (name, id) in created)
                byName[name] = id;
        }

        return byName;
    }
}