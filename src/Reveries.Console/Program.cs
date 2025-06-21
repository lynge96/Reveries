using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reveries.Application.Services;
using Reveries.Infrastructure.DependencyInjection;
using Reveries.Infrastructure.ISBNDB;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<IsbndbSettings>(context.Configuration.GetSection("Isbndb"));
        
        services.AddIsbndbClient();
        services.AddScoped<BookService>();
    })
    .Build();

var bookService = host.Services.GetRequiredService<BookService>();
var book = await bookService.GetBookByIsbnAsync("9780804139021");

Console.WriteLine(book?.Title);