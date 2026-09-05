using NSubstitute;
using Reveries.Application.Books.Services;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Works;

namespace Reveries.Application.Tests.Books;

public class GenreResolverTests
{
    private readonly IGenreRepository _genres = Substitute.For<IGenreRepository>();

    [Fact]
    public async Task ResolveIdsAsync_returns_empty_and_touches_nothing_for_empty_input()
    {
        var resolver = new GenreResolver(_genres);

        var result = await resolver.ResolveIdsAsync([]);

        Assert.Empty(result);
        await _genres.DidNotReceive().GetByNamesAsync(Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResolveIdsAsync_inserts_only_missing_genres_and_merges_ids()
    {
        var genres = new[] { Genre.TryCreate("Dystopia")!, Genre.TryCreate("Fantasy")! };
        _genres.GetByNamesAsync(Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { ["Dystopia"] = 1 });
        _genres.AddRangeAsync(Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { ["Fantasy"] = 2 });

        var resolver = new GenreResolver(_genres);
        var result = await resolver.ResolveIdsAsync(genres);

        await _genres.Received(1).AddRangeAsync(
            Arg.Is<IReadOnlyList<string>>(n => n.Count == 1 && n[0] == "Fantasy"),
            Arg.Any<CancellationToken>());
        Assert.Equal(1, result["Dystopia"]);
        Assert.Equal(2, result["Fantasy"]);
    }

    [Fact]
    public async Task ResolveIdsAsync_does_not_insert_when_all_genres_exist()
    {
        var genres = new[] { Genre.TryCreate("Dystopia")! };
        _genres.GetByNamesAsync(Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { ["Dystopia"] = 1 });

        var resolver = new GenreResolver(_genres);
        var result = await resolver.ResolveIdsAsync(genres);

        Assert.Equal(1, result["Dystopia"]);
        await _genres.DidNotReceive().AddRangeAsync(Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>());
    }
}