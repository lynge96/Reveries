namespace Reveries.Core.ValueObjects;

public sealed record Title
{
    public string Value { get; }

    private const int MaxLength = 500;

    internal Title(string title) => Value = title;
    
    public static Title Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Title cannot be empty.", nameof(value));
        
        var trimmed = value.Trim();
        
        if (trimmed.Length > MaxLength)
            throw new ArgumentException($"Title cannot exceed {MaxLength} characters.", nameof(value));
        
        return new Title(trimmed);
    }
    
    public override string ToString() => Value;
}