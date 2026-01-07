namespace Reveries.Application.Extensions;

public static class LoggingStringExtensions
{
    public static string TruncateForLog(this string? value, int maxLength = 600)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return value!.Length > maxLength
            ? value.Substring(0, maxLength) + "..."
            : value;
    }
}