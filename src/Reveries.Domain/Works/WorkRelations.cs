namespace Reveries.Domain.Works;

public sealed record WorkRelations(
    IReadOnlyList<int> PrimaryGenreIds,
    IReadOnlyList<int> SecondaryGenreIds,
    IReadOnlyList<int> DeweyDecimalIds);
