using System.Text;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reveries.Application.Services;
using Reveries.Console.Handlers;
using Reveries.Console.Handlers.Interfaces;
using Reveries.Console.Services;
using Reveries.Console.Services.Interfaces;
using Reveries.Infrastructure.Extensions;
using Reveries.Integration.GoogleBooks.Extensions;
using Reveries.Integration.Isbndb.Extensions;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((config) =>
    {
        Env.Load();
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddInfrastructure(context.Configuration);
        services.AddApplication();
        services.AddIsbndbServices();
        services.AddGoogleBooksServices();

        services.AddTransient<IConsoleAppRunnerService, ConsoleAppRunnerService>();
        services.AddScoped<IMenuOperationService, MenuOperationService>();
        services.AddScoped<IBookSaveService, BookSaveService>();
        services.AddScoped<IBookSaveService, BookSaveService>();
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