using Reveries.Application.Common.Validation.Exceptions;

namespace Reveries.Application.Common.Validation;

public record IsbnValidationResult(
    List<string> ValidIsbns
);

public static class IsbnValidationHelper
{
    public static string ValidateSingleIsbn(string isbn)
    {
        var normalizedIsbn = IsbnValidator.Normalize(isbn);
        if (!IsbnValidator.IsValid(normalizedIsbn))
            throw new IsbnValidationException($"Invalid ISBN checksum for '{isbn}'. Please verify the number is correct.", nameof(isbn));
        
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
            throw new IsbnValidationException(
                $"The following ISBN numbers are invalid: {string.Join(", ", invalidIsbns)}. Invalid ISBN checksum.");
        }

        return new IsbnValidationResult(validIsbns);
    }

}
