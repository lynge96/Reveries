using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reveries.Application.Extensions;
using Reveries.Console.Interfaces;
using Reveries.Console.Services;
using Reveries.Console.Services.Handlers;
using Reveries.Infrastructure.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddInfrastructure(context.Configuration);
        services.AddApplication();

        services.AddTransient<IConsoleAppRunner, ConsoleAppRunner>();
        services.AddScoped<IMenuOperationService, MenuOperationService>();
        services.AddScoped<IMenuHandler, SearchBookHandler>();

    })
    .Build();

Console.OutputEncoding = System.Text.Encoding.UTF8;

var runner = host.Services.GetRequiredService<IConsoleAppRunner>();

await runner.RunAsync();