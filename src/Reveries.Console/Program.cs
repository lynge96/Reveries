using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reveries.Application.DependencyInjection;
using Reveries.Application.Services;
using Reveries.Infrastructure.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddInfrastructure(context.Configuration);
        services.AddApplication();
    })
    .Build();

var bookService = host.Services.GetRequiredService<BookService>();

var book = await bookService.GetBookByIsbnAsync("9780804139021");

Console.WriteLine(book?.Title);