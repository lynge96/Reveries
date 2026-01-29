namespace Reveries.Core.Models;

public class DeweyDecimal
{
    public string Code { get; }
    
    private DeweyDecimal(string code)
    {
        Code = code;
    }
    
    public static DeweyDecimal? Create(string? rawCode)
    {
        var normalized = Normalize(rawCode);
        
        return normalized is null ? null : new DeweyDecimal(normalized);
    }
    
    private static string? Normalize(string? code)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;

        var normalized = code.Trim();
        
        if (normalized.Contains("/."))
            normalized = normalized.Replace("/.", ".");
        
        var slashIndex = normalized.IndexOf('/');
        if (slashIndex > 0)
        {
            var afterSlash = normalized[(slashIndex + 1)..];
            if (afterSlash.Length > 0 && char.IsDigit(afterSlash[0]))
            {
                normalized = normalized[..slashIndex];
            }
        }

        normalized = normalized.TrimEnd('.');
        
        return normalized.Length > 0 ? normalized : null;
    }
}