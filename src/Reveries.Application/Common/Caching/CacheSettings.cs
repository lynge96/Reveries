namespace Reveries.Application.Common.Caching;

public sealed class CacheSettings
{
    public const string SectionName = "Cache";

    public int IsbnLookupTtlHours { get; init; } = 12;
}
