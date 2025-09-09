using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Services.Isbndb;
using Reveries.Core.Entities.Settings;
using Reveries.Integration.Isbndb.Clients;
using Reveries.Integration.Isbndb.Configuration;

namespace Reveries.Integration.Isbndb.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIsbndbServices(this IServiceCollection services)
    {
        services.Configure<IsbndbSettings>(options =>
        {
            options.ApiKey = Environment.GetEnvironmentVariable("ISBNDB_API_KEY") 
                             ?? throw new InvalidOperationException("ISBNDB_API_KEY missing");
            options.ApiUrl = Environment.GetEnvironmentVariable("ISBNDB_API_URL") 
                             ?? throw new InvalidOperationException("ISBNDB_API_URL missing");
        });
        
        services.AddHttpClient();
        services.AddIsbndb();
        services.AddScoped<IIsbndbBookService, IsbndbBookService>();
        
        return services;
    }
}
