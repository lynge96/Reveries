using Reveries.Domain.Authors;

namespace Reveries.Domain.Works;

public sealed record WorkData(
    string Title,
    string? Subtitle,
    IReadOnlyList<AuthorId>? AuthorIds,
    IEnumerable<string>? PrimaryGenres,
    IEnumerable<string>? SecondaryGenres,
    IEnumerable<string>? DeweyDecimals,
    string? Synopsis,
    string? Description);