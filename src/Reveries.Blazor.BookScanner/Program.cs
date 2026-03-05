using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using Reveries.Blazor.BookScanner;
using Reveries.Blazor.BookScanner.Clients;
using Reveries.Blazor.BookScanner.Models;
using Reveries.Blazor.BookScanner.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection("Api"));

builder.Services.AddScoped(sp =>
{
    var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;

    return new HttpClient
    {
        BaseAddress = new Uri(options.BaseUrl)
    };
});

builder.Services.AddScoped<BookApiClient>();
builder.Services.AddScoped<BookState>();

await builder.Build().RunAsync();
