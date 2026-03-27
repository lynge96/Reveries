using Reveries.Application.Interfaces.Authors;
using Reveries.Core.Models;

namespace Reveries.Application.Services.Authors;

public class AuthorEnrichmentService
{
    private readonly IAuthorSearch _authorSearch;
    const int MaxCacheSize = 1000;
    
    public AuthorEnrichmentService(IAuthorSearch authorSearch)
    {
        _authorSearch = authorSearch;
    }
    
    public async Task EnrichAsync(IReadOnlyList<Author> authors, CancellationToken ct)
    {
        var variantsCache = new Dictionary<string, IReadOnlyList<Author>>();

        foreach (var author in authors)
        {
            var normalizedName = author.NormalizedName;
            
            if (!variantsCache.TryGetValue(normalizedName, out var variants))
            {
                variants = await _authorSearch.GetAuthorsByNameAsync(normalizedName, ct) ?? [];
                
                if (variantsCache.Count >= MaxCacheSize)
                {
                    var oldestKey = variantsCache.Keys.First();
                    variantsCache.Remove(oldestKey);
                }

                variantsCache[normalizedName] = variants;
            }
            
            if (!author.NameVariants.Any())
            {
                author.AddNameVariant(normalizedName, makePrimary: true);
            }
            
            foreach (var variant in variants)
            {
                if (!variant.NormalizedName.Equals(normalizedName, StringComparison.OrdinalIgnoreCase))
                {
                    author.AddNameVariant(variant.NormalizedName, makePrimary: false);
                }
            }
        }
    }
}