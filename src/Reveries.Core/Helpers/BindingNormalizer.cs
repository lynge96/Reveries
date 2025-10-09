namespace Reveries.Core.Helpers;

public static class BindingNormalizer
{
    private static readonly Dictionary<string, string> BindingMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "paperback", "Paperback" },
        { "softcover", "Paperback" },
        { "soft cover", "Paperback" },
        { "trade paperback", "Paperback" },
        { "pb", "Paperback" },
        { "mass market paperback", "Paperback" },

        { "hardcover", "Hardback" },
        { "hard cover", "Hardback" },
        { "hardback", "Hardback" },
        { "hb", "Hardback" }
    };

    public static string NormalizeBinding(this string? rawBinding)
    {
        if (string.IsNullOrWhiteSpace(rawBinding))
            return "Unknown";

        var normalized = rawBinding.Trim().ToLowerInvariant();
        
        if (BindingMap.TryGetValue(normalized, out var standard))
            return standard;
        
        if (normalized.Contains("paper"))
            return "Paperback";
        if (normalized.Contains("hard"))
            return "Hardcover";

        return "Unknown";
    }
}