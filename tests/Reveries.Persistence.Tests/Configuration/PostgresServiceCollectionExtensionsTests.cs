using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reveries.Persistence.Configuration;
using Reveries.Persistence.Exceptions;

namespace Reveries.Persistence.Tests.Configuration;

/// <summary>
/// Unit tests for the Postgres DI registration guard. These need no database, so the
/// class stays out of the container-backed test collection.
/// </summary>
public class PostgresServiceCollectionExtensionsTests
{
    [Fact]
    public void AddPostgres_throws_when_the_connection_string_is_missing()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder().Build();

        // Act
        var exception = Assert.Throws<MissingConnectionStringException>(
            () => services.AddPostgres(config));

        // Assert
        Assert.Equal("ReveriesDb", exception.Name);
    }

    [Fact]
    public void AddPostgres_does_not_throw_when_the_connection_string_is_present()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:ReveriesDb"] = "Host=localhost;Database=reveries"
            })
            .Build();

        // Act + Assert
        var exception = Record.Exception(() => services.AddPostgres(config));
        Assert.Null(exception);
    }
}