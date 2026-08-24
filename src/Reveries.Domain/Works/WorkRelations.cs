using Reveries.Domain.Authors;

namespace Reveries.Domain.Works;

public sealed record WorkRelations(
    IReadOnlyList<AuthorId> AuthorIds,
    IReadOnlyList<int> PrimaryGenreIds,
    IReadOnlyList<int> SecondaryGenreIds,
    IReadOnlyList<int> DeweyDecimalIds);