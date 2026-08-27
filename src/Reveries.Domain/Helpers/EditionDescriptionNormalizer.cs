using System.Text;

namespace Reveries.Domain.Helpers;

public static class EditionDescriptionNormalizer
{
    public static string? Normalize(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var builder = new StringBuilder(raw.Length);
        var previousWasSpace = false;

        foreach (var character in raw)
        {
            if (char.IsWhiteSpace(character))
            {
                if (builder.Length > 0)
                    previousWasSpace = true;
            }
            else if (!char.IsControl(character))
            {
                if (previousWasSpace)
                    builder.Append(' ');

                builder.Append(character);
                previousWasSpace = false;
            }
        }

        return builder.Length == 0 ? null : builder.ToString();
    }
}
