using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Reveries.Blazor.BookScanner;
using Reveries.Blazor.BookScanner.Clients;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7238/api/v1/")
});

builder.Services.AddScoped<BookApiClient>();

await builder.Build().RunAsync();
