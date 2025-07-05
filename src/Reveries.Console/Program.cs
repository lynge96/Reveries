using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reveries.Application.Extensions;
using Reveries.Console.Interfaces;
using Reveries.Console.Services;
using Reveries.Infrastructure.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddInfrastructure(context.Configuration);
        services.AddApplication();

        services.AddTransient<IConsoleAppRunner, ConsoleAppRunner>();
    })
    .Build();

Console.OutputEncoding = System.Text.Encoding.UTF8;

var consoleApp = host.Services.GetRequiredService<IConsoleAppRunner>();

await consoleApp.RunAsync();