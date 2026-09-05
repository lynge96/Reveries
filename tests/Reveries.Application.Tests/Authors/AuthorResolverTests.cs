using NSubstitute;
using Reveries.Application.Authors.Services;
using Reveries.Domain.Authors;
using Reveries.Domain.Interfaces.Repositories;

namespace Reveries.Application.Tests.Authors;

public class AuthorResolverTests
{
    private readonly IAuthorRepository _authors = Substitute.For<IAuthorRepository>();

    [Fact]
    public async Task ResolveIdsAsync_returns_empty_and_touches_nothing_for_empty_input()
    {
        var resolver = new AuthorResolver(_authors);

        var result = await resolver.ResolveIdsAsync([]);

        Assert.Empty(result);
        await _authors.DidNotReceive().GetByNamesAsync(Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>());
        await _authors.DidNotReceive().AddRangeAsync(Arg.Any<IReadOnlyList<Author>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResolveIdsAsync_does_not_insert_when_all_authors_exist()
    {
        var orwell = Author.TryCreate("George Orwell")!;
        _authors.GetByNamesAsync(Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Author> { orwell });

        var resolver = new AuthorResolver(_authors);
        var result = await resolver.ResolveIdsAsync([orwell]);

        Assert.Equal(new[] { orwell.Id }, result);
        await _authors.DidNotReceive().AddRangeAsync(Arg.Any<IReadOnlyList<Author>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResolveIdsAsync_inserts_only_the_missing_authors()
    {
        var existing = Author.TryCreate("George Orwell")!;
        var missing = Author.TryCreate("Aldous Huxley")!;
        _authors.GetByNamesAsync(Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Author> { existing });

        var resolver = new AuthorResolver(_authors);
        var result = await resolver.ResolveIdsAsync([existing, missing]);

        await _authors.Received(1).AddRangeAsync(
            Arg.Is<IReadOnlyList<Author>>(a => a.Count == 1 && a[0].Name == "Aldous Huxley"),
            Arg.Any<CancellationToken>());
        Assert.Equal(new[] { existing.Id, missing.Id }, result);
    }

    [Fact]
    public async Task ResolveIdsAsync_deduplicates_repeated_authors()
    {
        _authors.GetByNamesAsync(Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Author>());
        var first = Author.TryCreate("George Orwell")!;
        var duplicate = Author.TryCreate("george orwell")!;

        var resolver = new AuthorResolver(_authors);
        var result = await resolver.ResolveIdsAsync([first, duplicate]);

        Assert.Single(result);
        await _authors.Received(1).AddRangeAsync(
            Arg.Is<IReadOnlyList<Author>>(a => a.Count == 1),
            Arg.Any<CancellationToken>());
    }
}
