namespace Reveries.Domain.Works;

public sealed record WorkData(
    string Title,
    IEnumerable<string>? Authors,
    IEnumerable<string>? PrimaryGenres,
    IEnumerable<string>? SecondaryGenres,
    IEnumerable<string>? DeweyDecimals,
    string? Synopsis,
    string? Description);