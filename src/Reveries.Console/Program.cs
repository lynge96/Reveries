using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reveries.Application.Services;
using Reveries.Console.Handlers;
using Reveries.Console.Handlers.Interfaces;
using Reveries.Console.Services;
using Reveries.Console.Services.Interfaces;
using Reveries.Infrastructure.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddInfrastructure(context.Configuration);
        services.AddApplication();

        services.AddTransient<IConsoleAppRunnerService, ConsoleAppRunnerService>();
        services.AddScoped<IMenuOperationService, MenuOperationService>();
        services.AddScoped<IBookSaveService, BookSaveService>();
        services.AddScoped<IBookSaveService, BookSaveService>();
        services.AddScoped<IBookDisplayService, BookDisplayService>();
        services.AddScoped<IBookSelectionService, BookSelectionService>();
        services.AddScoped<IMenuHandler, SearchBookHandler>();
        services.AddScoped<IMenuHandler, SearchAuthorHandler>();
        services.AddScoped<IMenuHandler, SearchPublisherHandler>();
    })
    .Build();

Console.OutputEncoding = Encoding.UTF8;

var runner = host.Services.GetRequiredService<IConsoleAppRunnerService>();

await runner.RunAsync();