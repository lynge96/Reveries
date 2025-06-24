using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Services;

namespace Reveries.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<BookService>();
        
        return services;
    }
}