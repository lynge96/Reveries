using System.Diagnostics;
using System.Text.RegularExpressions;
using Reveries.Domain.Enums;

namespace Reveries.Domain.Works;

public sealed partial record DeweyDecimal
{
    public string Code { get; }
    public DeweyClass MainCategory => (DeweyClass)int.Parse(Code[..1]);
    public string MainCategoryName => MainCategory switch
    {
        DeweyClass.General => "Computer science, information & general works",
        DeweyClass.Philosophy => "Philosophy & psychology",
        DeweyClass.Religion => "Religion",
        DeweyClass.SocialSciences => "Social sciences",
        DeweyClass.Language => "Language",
        DeweyClass.Science => "Science",
        DeweyClass.Technology => "Technology",
        DeweyClass.Arts => "Arts & recreation",
        DeweyClass.Literature => "Literature",
        DeweyClass.History => "History & geography",
        _ => throw new UnreachableException($"Unmapped Dewey class: {MainCategory}")
    };

    private DeweyDecimal(string code)
    {
        Code = code;
    }

    public static DeweyDecimal? TryCreate(string? rawCode)
    {
        if (string.IsNullOrWhiteSpace(rawCode))
            return null;

        var normalized = Normalize(rawCode);

        if (!DeweyPattern().IsMatch(normalized))
            return null;

        return new DeweyDecimal(normalized);
    }

    public static DeweyDecimal Reconstitute(string code) => new(code);

    public override string ToString() => Code;

    private static string Normalize(string code)
    {
        var normalized = code.Trim().Replace("/.", ".");

        var slashIndex = normalized.IndexOf('/');
        if (slashIndex > 0)
            normalized = normalized[..slashIndex];

        return normalized.TrimEnd('.');
    }

    [GeneratedRegex(@"^\d{1,3}(?:\.\d+)?$")]
    private static partial Regex DeweyPattern();
}