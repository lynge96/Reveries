using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Services;
using Reveries.Core.Models;

namespace Reveries.Application.Services;

public class AuthorEnrichmentService : IAuthorEnrichmentService
{
    private readonly IIsbndbAuthorService _isbndbAuthorService;
    
    public AuthorEnrichmentService(IIsbndbAuthorService isbndbAuthorService)
    {
        _isbndbAuthorService = isbndbAuthorService;
    }
    
    public async Task EnrichAsync(IReadOnlyList<Author> authors, CancellationToken ct)
    {
        var variantsCache = new Dictionary<string, IReadOnlyList<Author>>();

        foreach (var author in authors)
        {
            var normalizedName = author.NormalizedName;
            
            if (!variantsCache.TryGetValue(normalizedName, out var variants))
            {
                variants = await _isbndbAuthorService.GetAuthorsByNameAsync(normalizedName, ct);

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