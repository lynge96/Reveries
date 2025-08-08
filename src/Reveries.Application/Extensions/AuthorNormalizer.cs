namespace Reveries.Application.Extensions;

public static class AuthorNameNormalizer
{
    public static (string FirstName, string LastName, string NormalizedName) NormalizeAuthorName(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
            return (string.Empty, string.Empty, string.Empty);

        rawName = rawName.Trim().Replace(';', ' ');
    
        string firstName;
        string lastName;

        if (rawName.Contains(',')) // Format: Lastname, Firstname
        {
            var parts = rawName.Split(',', StringSplitOptions.TrimEntries);
            lastName = parts[0];
            firstName = parts.Length > 1 ? parts[1] : string.Empty;
        }
        else // Format: Firstname; Lastname (<space> or ;)
        {
            var parts = rawName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            firstName = parts[0];
            lastName = parts.Length > 1 ? parts[^1] : string.Empty;
        }

        var normalizedName = $"{firstName} {lastName}".Trim().ToLowerInvariant();
        
        return (firstName, lastName, normalizedName);
    }
}