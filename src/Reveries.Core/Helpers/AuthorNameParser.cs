namespace Reveries.Core.Helpers;

public static class AuthorNameNormalizer
{
    public static (string FirstName, string LastName, string NormalizedName) Parse(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
            return (string.Empty, string.Empty, string.Empty);

        // Normalisering: trim og erstat semikolon med mellemrum
        var cleanedName = rawName.Trim().Replace(';', ' ');

        // Bestem format ud fra komma
        var (firstName, lastName) = cleanedName.Contains(',')
            ? ParseLastNameFirst(cleanedName)
            : ParseFirstNameFirst(cleanedName);

        var normalizedName = $"{firstName} {lastName}".Trim().ToLowerInvariant();
        
        return (firstName, lastName, normalizedName);
    }
    
    /// Parser format: "Lastname, Firstname Middlename"
    private static (string firstName, string lastName) ParseLastNameFirst(string name)
    {
        var parts = name.Split(',', StringSplitOptions.TrimEntries);
        var lastName = parts[0];
        var firstName = parts.Length > 1 ? parts[1] : string.Empty;
        return (firstName, lastName);
    }
    
    /// Parser format: "Firstname Middlename Lastname"
    private static (string firstName, string lastName) ParseFirstNameFirst(string name)
    {
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        return parts.Length switch
        {
            1 => (parts[0], string.Empty),
            _ => (string.Join(" ", parts.Take(parts.Length - 1)), parts[parts.Length - 1])
        };
    }
}