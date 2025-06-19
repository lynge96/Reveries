using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reveries.Application.Interfaces;
using Reveries.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

var apiKey = builder.Configuration["Isbndb:ApiKey"];

builder.Services.AddIsbndbClient(apiKey);

var app = builder.Build();

using var scope = app.Services.CreateScope();
var client = scope.ServiceProvider.GetRequiredService<IIsbndbClient>();

var book = await client.GetBookByIsbnAsync("9780804139021");

// Console.WriteLine(book?.Title ?? "Bog ikke fundet.");