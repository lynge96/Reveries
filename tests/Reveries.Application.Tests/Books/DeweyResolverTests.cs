using NSubstitute;
using Reveries.Application.Books.Services;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Works;

namespace Reveries.Application.Tests.Books;

public class DeweyResolverTests
{
    private readonly IDeweyDecimalsRepository _dewey = Substitute.For<IDeweyDecimalsRepository>();

    [Fact]
    public async Task ResolveIdsAsync_returns_empty_and_touches_nothing_for_empty_input()
    {
        var resolver = new DeweyResolver(_dewey);

        var result = await resolver.ResolveIdsAsync([]);

        Assert.Empty(result);
        await _dewey.DidNotReceive().GetByCodesAsync(Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResolveIdsAsync_inserts_only_missing_codes_and_returns_all_ids()
    {
        var codes = new[] { DeweyDecimal.TryCreate("823")!, DeweyDecimal.TryCreate("813")! };
        _dewey.GetByCodesAsync(Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<string, int> { ["823"] = 10 });
        _dewey.AddRangeAsync(Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<string, int> { ["813"] = 11 });

        var resolver = new DeweyResolver(_dewey);
        var result = await resolver.ResolveIdsAsync(codes);

        await _dewey.Received(1).AddRangeAsync(
            Arg.Is<IReadOnlyList<string>>(c => c.Count == 1 && c[0] == "813"),
            Arg.Any<CancellationToken>());
        Assert.Equal(new[] { 10, 11 }, result);
    }
}