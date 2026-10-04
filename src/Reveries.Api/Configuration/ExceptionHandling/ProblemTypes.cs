using System.Text;

namespace Reveries.Api.Configuration.ExceptionHandling;

internal static class ProblemTypes
{
    private const string BaseUri = "https://reveries.dk/errors/";

    public static string UriFor(string errorCode)
    {
        return BaseUri + errorCode;
    }

    public static string ToErrorCode(string errorType)
    {
        const string suffix = "Exception";

        var name = errorType.EndsWith(suffix, StringComparison.Ordinal)
            ? errorType[..^suffix.Length]
            : errorType;

        var builder = new StringBuilder(name.Length + 8);
        for (var i = 0; i < name.Length; i++)
        {
            var character = name[i];
            if (char.IsUpper(character) && i > 0)
                builder.Append('-');

            builder.Append(char.ToLowerInvariant(character));
        }

        return builder.ToString();
    }
}