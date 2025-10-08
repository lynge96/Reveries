namespace Reveries.Core.Helpers;

public static class AuthorNameNormalizer
{
    public static (string FirstName, string LastName, string NormalizedName) NormalizeAuthorName(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
            return (string.Empty, string.Empty, string.Empty);

        rawName = rawName.Trim().Replace(';', ' ');

        var firstName = string.Empty;
        var lastName = string.Empty;

        if (rawName.Contains(',')) // Format: Lastname, Firstname Middlename
        {
            var parts = rawName.Split(',', StringSplitOptions.TrimEntries);
            lastName = parts[0];
            firstName = parts.Length > 1 ? parts[1] : string.Empty;
        }
        else // Format: Firstname Middlename Lastname
        {
            var parts = rawName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                firstName = parts[0];
            }
            else if (parts.Length >= 2)
            {
                firstName = parts[0];
                lastName = parts[^1];
                // Behold evt. mellemnavne som en del af fornavnet
                if (parts.Length > 2)
                {
                    firstName = string.Join(" ", parts.Take(parts.Length - 1));
                }
            }
        }

        var normalizedName = $"{firstName} {lastName}".Trim().ToLowerInvariant();

        return (firstName, lastName, normalizedName);
    }
}