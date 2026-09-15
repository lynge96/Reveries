using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reveries.Persistence.Configuration;

namespace Reveries.Persistence.Tests.Configuration;

/// <summary>
/// Unit tests for the Postgres DI registration guard. Registration itself never throws
/// (so design-time tooling can build the service graph without a database); a missing
/// connection string is caught by options validation on start instead. These need no
/// database, so the class stays out of the container-backed test collection.
/// </summary>
public class PostgresServiceCollectionExtensionsTests
{
    [Fact]
    public void AddPostgres_fails_validation_when_the_connection_string_is_missing()
    {
        // Arrange — registration must not throw; the guard runs when the options resolve.
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder().Build();
        services.AddPostgres(config);
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<ReveriesDbOptions>>();

        // Act
        var exception = Assert.Throws<OptionsValidationException>(() => _ = options.Value);

        // Assert
        Assert.Contains("ReveriesDb", exception.Message);
    }

    [Fact]
    public void AddPostgres_passes_validation_when_the_connection_string_is_present()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:ReveriesDb"] = "Host=localhost;Database=reveries"
            })
            .Build();
        services.AddPostgres(config);
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<ReveriesDbOptions>>();

        // Act + Assert
        var exception = Record.Exception(() => _ = options.Value);
        Assert.Null(exception);
    }
}