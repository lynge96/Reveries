using System.Text.RegularExpressions;

namespace Reveries.Core.Helpers;

public static partial class AuthorNameNormalizer
{
    [GeneratedRegex(@"\s+")]
    private static partial Regex MultipleWhitespaceRegex();
    
    [GeneratedRegex(@"[^\p{L}\s,'-]")]
    private static partial Regex SpecialCharsRegex();
    
    public static (string FirstName, string LastName) Parse(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
            return (string.Empty, string.Empty);
        
        var cleanedName = SpecialCharsRegex().Replace(rawName, " ");
        
        cleanedName = MultipleWhitespaceRegex().Replace(cleanedName, " ").Trim();
        
        var (firstName, lastName) = cleanedName.Contains(',')
            ? ParseLastNameFirst(cleanedName)
            : ParseFirstNameFirst(cleanedName);

        return (firstName.Trim(), lastName.Trim());
    }
    
    /// Parser format: "Lastname, Firstname Middlename"
    private static (string firstName, string lastName) ParseLastNameFirst(string name)
    {
        var parts = name.Split(',', 2, StringSplitOptions.TrimEntries);
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
            0 => (string.Empty, string.Empty),
            1 => (parts[0], string.Empty),
            _ => (string.Join(" ", parts[..^1]), parts[^1])
        };
    }
}