using System.Text;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reveries.Application;
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

        services.AddApplication();
        services.AddRedisCache(configuration);
        services.AddPostgresql(configuration);
        services.AddIsbndb(configuration);
        services.AddGoogleBooks(configuration);
        
        services.AddTransient<IConsoleAppRunnerService, ConsoleAppRunnerService>();
        
        services.AddScoped<IMenuOperationService, MenuOperationService>();
        
        services.AddScoped<BookSelectionService>();
        services.AddScoped<SaveEntityService>();
        services.AddScoped<BookDisplayService>();
        
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