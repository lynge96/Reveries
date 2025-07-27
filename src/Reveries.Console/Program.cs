using System.Text;
using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reveries.Application.Extensions;
using Reveries.Console.Features.Console.Interfaces;
using Reveries.Console.Features.Console.Services;
using Reveries.Console.Features.Handlers;
using Reveries.Console.Features.Handlers.Interfaces;
using Reveries.Infrastructure.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddInfrastructure(context.Configuration);
        services.AddApplication();

        services.AddTransient<IConsoleAppRunner, ConsoleAppRunner>();
        services.AddScoped<IMenuOperationService, MenuOperationService>();
        services.AddScoped<IMenuHandler, SearchBookHandler>();
        services.AddScoped<IMenuHandler, SearchAuthorHandler>();
        services.AddScoped<IMenuHandler, SearchPublisherHandler>();
    })
    .Build();

Console.OutputEncoding = Encoding.UTF8;

var runner = host.Services.GetRequiredService<IConsoleAppRunner>();

await runner.RunAsync();