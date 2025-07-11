namespace Reveries.Application.Common.Validation;

public record IsbnValidationResult(
    List<string> ValidIsbns, 
    List<string> InvalidIsbns);


public static class IsbnValidationHelper
{
    public static string ValidateSingleIsbn(string isbn)
    {
        var normalizedIsbn = IsbnValidator.Normalize(isbn);
        if (!IsbnValidator.IsValid(normalizedIsbn))
            throw new ArgumentException("ISBN must be either 10 or 13 digits.", nameof(isbn));
        
        return normalizedIsbn;
    }

    public static IsbnValidationResult ValidateIsbns(IEnumerable<string> isbns)
    {
        var validIsbns = new List<string>();
        var invalidIsbns = new List<string>();

        foreach (var isbn in isbns)
        {
            var normalizedIsbn = IsbnValidator.Normalize(isbn);
            (IsbnValidator.IsValid(normalizedIsbn) ? validIsbns : invalidIsbns)
                .Add(IsbnValidator.IsValid(normalizedIsbn) ? normalizedIsbn : isbn);
        }

        if (invalidIsbns.Any())
        {
            throw new ArgumentException(
                $"The following ISBN numbers are invalid: {string.Join(", ", invalidIsbns)}");
        }

        return new IsbnValidationResult(validIsbns, invalidIsbns);
    }

}
