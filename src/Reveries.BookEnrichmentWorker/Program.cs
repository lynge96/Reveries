using DotNetEnv;
using Reveries.Application.Services;
using Reveries.BookEnrichmentWorker;
using Reveries.Infrastructure.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        Env.Load();
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplication();
        services.AddInfrastructure(context.Configuration);

        services.AddSingleton<BookEnrichmentRunner>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();