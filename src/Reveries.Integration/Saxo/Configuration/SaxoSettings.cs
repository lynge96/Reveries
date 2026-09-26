namespace Reveries.Integration.Saxo.Configuration;

public sealed class SaxoSettings
{
    public const string SectionName = "Saxo";

    public string SearchUrlTemplate { get; init; } = "https://www.saxo.com/dk/products/search?query={0}";
}