using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Services;

namespace Reveries.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        
        return services;
    }
}