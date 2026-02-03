namespace Reveries.Core.ValueObjects;

public sealed record DeweyDecimal
{
    public string Code { get; }
    
    private DeweyDecimal(string code)
    {
        Code = code;
    }
    
    public static DeweyDecimal Create(string rawCode)
    {
        var normalized = Normalize(rawCode);
        
        return new DeweyDecimal(normalized);
    }
    
    private static string Normalize(string code)
    {
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
        
        return normalized;
    }
}