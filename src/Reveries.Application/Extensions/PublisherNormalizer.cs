namespace Reveries.Application.Extensions;

public class PublisherNormalizer
{
    private static readonly char[] TrimChars = { ',', ' ', ':', ';', '.' };
    
    public static string NormalizePublisher(string publisher)
    {
        // Konvertér til title case og trim specialtegn
        var normalized = publisher.Trim(TrimChars);
        
        // Fjern parenteser og deres indhold
        normalized = System.Text.RegularExpressions.Regex.Replace(normalized, @"\s*\([^)]*\)", "");
        
        // Fjern alt efter "/" da det ofte er underforlag
        if (normalized.Contains('/'))
        {
            normalized = normalized.Split('/')[0].Trim();
        }
        
        // Fjern præfikser som "London :"
        normalized = System.Text.RegularExpressions.Regex.Replace(normalized, @"^[A-Za-z]+\s*:", "").Trim();
        
        return normalized.ToUpperInvariant();
    }
    
    public static IEnumerable<string> GetUniquePublishers(IEnumerable<string> publishers)
    {
        return publishers
            .Select(NormalizePublisher)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Distinct()
            .OrderBy(p => p);
    }
}