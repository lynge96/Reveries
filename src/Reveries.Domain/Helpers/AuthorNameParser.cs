using System.Text.RegularExpressions;

namespace Reveries.Core.Helpers;

public static partial class AuthorNameNormalizer
{
    [GeneratedRegex(@"\s+")]
    private static partial Regex MultipleWhitespaceRegex();
    
    [GeneratedRegex(@"[^\p{L}\s,'\.\-]")]
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
    
    /// Parser format: "Firstname Middlename Lastname" or "Initials Lastname"
    private static (string firstName, string lastName) ParseFirstNameFirst(string name)
    {
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length == 0)
            return (string.Empty, string.Empty);
        
        if (parts.Length == 1)
            return (parts[0], string.Empty);
        
        // Find the last part that doesn't end with a period (likely the last name)
        var lastNameIndex = -1;
        for (var i = parts.Length - 1; i >= 0; i--)
        {
            if (!parts[i].EndsWith('.'))
            {
                lastNameIndex = i;
                break;
            }
        }
        
        // If all parts end with period, or no valid lastname found
        if (lastNameIndex == -1 || lastNameIndex == 0)
            return (string.Join(" ", parts[..^1]), parts[^1]);
        
        // Split at the last non-initial part
        var firstName = string.Join(" ", parts[..lastNameIndex]);
        var lastName = parts[lastNameIndex];
        
        return (firstName, lastName);
    }
}