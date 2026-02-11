using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Reveries.Blazor.BookScanner;
using Reveries.Blazor.BookScanner.Clients;
using Reveries.Blazor.BookScanner.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7238/api/v1/")
});

builder.Services.AddScoped<BookApiClient>();
builder.Services.AddScoped<BookState>();

await builder.Build().RunAsync();
