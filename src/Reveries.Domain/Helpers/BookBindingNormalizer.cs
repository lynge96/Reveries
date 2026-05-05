using Reveries.Core.Enums;

namespace Reveries.Core.Helpers;

public static class BookBindingNormalizer
{
    private static readonly Dictionary<string, BindingType> BindingMap = new(StringComparer.OrdinalIgnoreCase)
    {
        // Paperback variants
        { "paperback", BindingType.Paperback },
        { "softcover", BindingType.Paperback },
        { "soft cover", BindingType.Paperback },
        { "trade paperback", BindingType.Paperback },
        { "pb", BindingType.Paperback },
        { "mass market paperback", BindingType.Paperback },
        { "tpb", BindingType.Paperback },

        // Hardback variants
        { "hardcover", BindingType.Hardback },
        { "hard cover", BindingType.Hardback },
        { "hardback", BindingType.Hardback },
        { "hb", BindingType.Hardback }
    };

    public static string GetStandardBinding(this string? rawBinding)
    {
        if (string.IsNullOrWhiteSpace(rawBinding))
            return nameof(BindingType.Unknown);

        var normalized = rawBinding.Trim().ToLowerInvariant();

        if (BindingMap.TryGetValue(normalized, out var bindingType))
            return bindingType.ToString();

        return normalized switch
        {
            _ when normalized.Contains("paper") => nameof(BindingType.Paperback),
            _ when normalized.Contains("hard") => nameof(BindingType.Hardback),
            _ => nameof(BindingType.Unknown)
        };
    }
}