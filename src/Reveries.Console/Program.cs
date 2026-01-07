using System.Text;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reveries.Application.Configuration;
using Reveries.Console.Handlers;
using Reveries.Console.Interfaces;
using Reveries.Console.Services;
using Reveries.Infrastructure.Postgresql.Configuration;
using Reveries.Infrastructure.Redis.Configuration;
using Reveries.Integration.GoogleBooks.Configuration;
using Reveries.Integration.Isbndb.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        Env.Load();
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddRedisCache(configuration);
        services.AddPostgresql(configuration);
        services.AddApplicationServices();
        services.AddIsbndbServices(configuration);
        services.AddGoogleBooksServices(configuration);
        services.AddTransient<IConsoleAppRunnerService, ConsoleAppRunnerService>();
        services.AddScoped<IMenuOperationService, MenuOperationService>();
        services.AddScoped<ISaveEntityService, SaveEntityEntityService>();
        services.AddScoped<IBookDisplayService, BookDisplayService>();
        services.AddScoped<IBookSelectionService, BookSelectionService>();
        services.AddScoped<IMenuHandler, SearchBookHandler>();
        services.AddScoped<IMenuHandler, SearchAuthorHandler>();
        services.AddScoped<IMenuHandler, SearchPublisherHandler>();
        services.AddScoped<IMenuHandler, DatabaseTableHandler>();
        services.AddScoped<IMenuHandler, BookSeriesHandler>();
    })
    .Build();

Console.OutputEncoding = Encoding.UTF8;

var runner = host.Services.GetRequiredService<IConsoleAppRunnerService>();

await runner.RunAsync();