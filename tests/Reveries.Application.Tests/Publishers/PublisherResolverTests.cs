using NSubstitute;
using Reveries.Application.Publishers.Services;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Publishers;

namespace Reveries.Application.Tests.Publishers;

public class PublisherResolverTests
{
    private readonly IPublisherRepository _publishers = Substitute.For<IPublisherRepository>();

    [Fact]
    public async Task ResolveAsync_returns_null_and_touches_nothing_for_null_input()
    {
        var resolver = new PublisherResolver(_publishers);

        var result = await resolver.ResolveAsync(null);

        Assert.Null(result);
        await _publishers.DidNotReceive().GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _publishers.DidNotReceive().AddAsync(Arg.Any<Publisher>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResolveAsync_returns_the_existing_publisher_without_inserting()
    {
        var existing = Publisher.TryCreate("Penguin")!;
        _publishers.GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(existing);

        var resolver = new PublisherResolver(_publishers);
        var result = await resolver.ResolveAsync(Publisher.TryCreate("Penguin"));

        Assert.Same(existing, result);
        await _publishers.DidNotReceive().AddAsync(Arg.Any<Publisher>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResolveAsync_inserts_and_returns_the_incoming_publisher_when_absent()
    {
        _publishers.GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Publisher?)null);
        var incoming = Publisher.TryCreate("Signet Classics")!;

        var resolver = new PublisherResolver(_publishers);
        var result = await resolver.ResolveAsync(incoming);

        Assert.Same(incoming, result);
        await _publishers.Received(1).AddAsync(incoming, Arg.Any<CancellationToken>());
    }
}