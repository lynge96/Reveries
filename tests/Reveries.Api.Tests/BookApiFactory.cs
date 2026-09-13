using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Reveries.Api;
using Reveries.Persistence.Migrations;

namespace Reveries.Api.Tests;

public sealed class BookApiFactory : WebApplicationFactory<IApiMarker>
{
    public IMediator Mediator { get; } = Substitute.For<IMediator>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting(
            "ConnectionStrings:ReveriesDb",
            "Host=localhost;Database=reveries_test;Username=test;Password=test");
        builder.UseSetting("Isbndb:ApiKey", "test-key");
        builder.UseSetting("GoogleBooks:ApiKey", "test-key");

        builder.ConfigureTestServices(services =>
        {
            RemoveDatabaseMigrations(services);
            services.AddSingleton(Mediator);
        });
    }

    private static void RemoveDatabaseMigrations(IServiceCollection services)
    {
        var migrationService = services.SingleOrDefault(descriptor =>
            descriptor.ImplementationType == typeof(DatabaseMigrationHostedService));

        if (migrationService is not null)
            services.Remove(migrationService);
    }
}