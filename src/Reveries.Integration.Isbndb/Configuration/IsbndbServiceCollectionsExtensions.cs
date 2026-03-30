using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Authors.Interfaces;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Publishers.Interfaces;
using Reveries.Integration.Isbndb.Services;

namespace Reveries.Integration.Isbndb.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIsbndbServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<IsbndbSettings>()
            .Bind(config.GetSection("Isbndb"))
            .Validate(s => !string.IsNullOrWhiteSpace(s.ApiUrl), "Isbndb: ApiUrl missing")
            .Validate(s => !string.IsNullOrWhiteSpace(s.ApiKey), "Isbndb: ApiKey missing")
            .Validate(s => s.MaxBulkIsbns > 0, "Isbndb: MaxBulkIsbns must be positive")
            .ValidateOnStart();
        
        services.AddIsbndbClients();

        services.AddScoped<IIsbndbBookSearch, IsbndbBookService>();
        services.AddScoped<IAuthorSearch, IsbndbAuthorService>();
        services.AddScoped<IPublisherSearch, IsbndbPublisherService>();
        
        return services;
    }
}
