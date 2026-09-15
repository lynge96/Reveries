using System.Reflection;
using NetArchTest.Rules;

namespace Reveries.Architecture.Tests;

/// <summary>
/// Asserts the Clean Architecture layer dependency rules on the compiled
/// namespaces. Each test fails with the names of the offending types.
/// </summary>
public class LayerDependencyTests
{
    private const string Domain = "Reveries.Domain";
    private const string Application = "Reveries.Application";
    private const string ApiContracts = "Reveries.Api.Contracts";
    private const string Infrastructure = "Reveries.Infrastructure";
    private const string Persistence = "Reveries.Persistence";
    private const string Integration = "Reveries.Integration";
    private const string Api = "Reveries.Api";

    private static readonly Assembly DomainAssembly = typeof(Domain.Works.Work).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(Application.ApplicationServiceCollectionExtensions).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.DependencyInjection).Assembly;
    private static readonly Assembly IntegrationAssembly = typeof(Integration.GoogleBooks.Clients.GoogleBooksClient).Assembly;
    private static readonly Assembly ApiAssembly = typeof(Api.Endpoints.BookEndpoints).Assembly;

    // Rule 1 — Domain depends on no outer layer.
    [Fact]
    public void Domain_should_not_depend_on_any_outer_layer()
    {
        // Arrange
        var types = Types.InAssembly(DomainAssembly);

        // Act
        var result = types
            .Should()
            .NotHaveDependencyOnAny(Application, Infrastructure, Persistence, Integration, Api)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, Describe(result));
    }

    // Rule 2 — Application depends on no outer adapter or the API boundary (the DTOs now live under Reveries.Api).
    [Fact]
    public void Application_should_depend_only_on_domain()
    {
        // Arrange
        var types = Types.InAssembly(ApplicationAssembly);

        // Act
        var result = types
            .Should()
            .NotHaveDependencyOnAny(Infrastructure, Persistence, Integration, Api)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, Describe(result));
    }

    // Rule 3 — the API contracts (DTOs, now a namespace inside Reveries.Api) expose no Domain type.
    [Fact]
    public void Contracts_should_not_depend_on_domain()
    {
        // Arrange
        var types = Types.InAssembly(ApiAssembly)
            .That()
            .ResideInNamespaceStartingWith(ApiContracts);

        // Act
        var result = types
            .Should()
            .NotHaveDependencyOn(Domain)
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, Describe(result));
    }

    // Rule 4 — no outer layer depends on the concrete Reveries.Persistence.Repositories types.
    [Fact]
    public void Concrete_repositories_should_not_leak_out_of_persistence()
    {
        // Arrange
        const string repositories = "Reveries.Persistence.Repositories";
        Assembly[] outerLayers = [ApplicationAssembly, InfrastructureAssembly, IntegrationAssembly, ApiAssembly];

        // Act
        var offenders = outerLayers
            .Select(assembly => Types.InAssembly(assembly)
                .Should()
                .NotHaveDependencyOn(repositories)
                .GetResult())
            .Where(result => !result.IsSuccessful)
            .SelectMany(result => result.FailingTypeNames ?? [])
            .ToList();

        // Assert
        Assert.True(offenders.Count == 0, $"Types depending on {repositories}: {string.Join(", ", offenders)}");
    }

    private static string Describe(TestResult result) =>
        "Offending types: " + string.Join(", ", result.FailingTypeNames ?? []);
}
