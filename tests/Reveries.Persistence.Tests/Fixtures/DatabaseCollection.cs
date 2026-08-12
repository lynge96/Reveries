namespace Reveries.Persistence.Tests.Fixtures;

/// <summary>
/// Shares one <see cref="PostgresContainerFixture"/> across every test in the
/// collection, so the container starts once rather than per test class.
/// </summary>
[CollectionDefinition(Name)]
public sealed class DatabaseCollection : ICollectionFixture<PostgresContainerFixture>
{
    public const string Name = "postgres-database";
}