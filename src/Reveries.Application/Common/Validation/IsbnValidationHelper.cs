using Reveries.Core.Exceptions;
using Reveries.Core.Validation;

namespace Reveries.Application.Common.Validation;

public static class IsbnValidationHelper
{
    public static List<string> ValidateIsbns(IEnumerable<string> isbns)
    {
        var valid = new List<string>();
        var invalid = new List<string>();

        foreach (var isbn in isbns)
        {
            if (IsbnValidator.TryValidate(isbn, out var normalized))
                valid.Add(normalized);
            else
                invalid.Add(isbn);
        }

        if (invalid.Count > 0)
            throw new IsbnValidationException($"Invalid ISBN(s): {string.Join(", ", invalid)}");

        return valid;
    }
}
