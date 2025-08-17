using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.Interfaces;
using Reveries.Core.Interfaces.Persistence;
using Reveries.Core.Interfaces.Repositories;
using Reveries.Core.Settings;
using Reveries.Infrastructure.Interfaces.Persistence;
using Reveries.Infrastructure.IsbnDb;
using Reveries.Infrastructure.Persistence;
using Reveries.Infrastructure.Persistence.Context;
using Reveries.Infrastructure.Persistence.Repositories;

namespace Reveries.Infrastructure.Services;

public static class InfrastructureServiceCollection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        Env.Load();

        var isbndbSettings = new IsbndbSettings
        {
            ApiKey = Environment.GetEnvironmentVariable("ISBNDB_API_KEY") ?? string.Empty,
            ApiUrl = Environment.GetEnvironmentVariable("ISBNDB_API_URL") ?? string.Empty
        };
        
        services.Configure<IsbndbSettings>(options =>
        {
            options.ApiKey = isbndbSettings.ApiKey;
            options.ApiUrl = isbndbSettings.ApiUrl;
        });

        services.AddIsbndbClients();
        services.AddScoped<IPostgresDbContext, PostgresDbContext>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IPublisherRepository, PublisherRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<IBookAuthorsRepository, BookAuthorsRepository>();
        services.AddScoped<IBookSubjectsRepository, BookSubjectsRepository>();
        services.AddScoped<IBookDimensionsRepository, BookDimensionsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
    
    private static IServiceCollection AddIsbndbClients(this IServiceCollection services)
    {
        services.AddHttpClient("Isbndb", (provider, client) =>
        {
            var settings = provider.GetRequiredService<IOptions<IsbndbSettings>>().Value;

            if (string.IsNullOrWhiteSpace(settings.ApiKey))
            {
                throw new InvalidOperationException("ISBNDB API key is not configured. Please add a valid API key in the .env file with ISBNDB_API_KEY");
            }

            if (string.IsNullOrWhiteSpace(settings.ApiUrl))
            {
                throw new InvalidOperationException("ISBNDB API URL is not configured. Please add a valid URL in the .env file with ISBNDB_API_URL");
            }


            client.BaseAddress = new Uri(settings.ApiUrl);
            client.DefaultRequestHeaders.Add("Authorization", settings.ApiKey);
        });
        
        services.AddTransient<IIsbndbBookClient, IsbndbBookClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            return new IsbndbBookClient(httpClientFactory.CreateClient("Isbndb"));
        });
        
        services.AddTransient<IIsbndbAuthorClient, IsbndbAuthorClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            return new IsbndbAuthorClient(httpClientFactory.CreateClient("Isbndb"));
        });
        
        services.AddTransient<IIsbndbPublisherClient, IsbndbPublisherClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            return new IsbndbPublisherClient(httpClientFactory.CreateClient("Isbndb"));
        });
        
        return services;
    }
}