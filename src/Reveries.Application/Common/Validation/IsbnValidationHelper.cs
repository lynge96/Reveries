using Reveries.Application.Common.Exceptions;

namespace Reveries.Application.Common.Validation;

public static class IsbnValidationHelper
{
    public static List<string> ValidateIsbns(IEnumerable<string> isbns)
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

        return validIsbns;
    }
}
