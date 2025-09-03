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
        services.AddIsbndb(configuration);
        services.AddPersistence();
        
        return services;
    }

}