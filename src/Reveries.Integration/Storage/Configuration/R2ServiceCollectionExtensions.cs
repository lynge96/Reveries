using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Reveries.Integration.Storage.Configuration;

public static class R2ServiceCollectionExtensions
{
    public static IServiceCollection AddR2(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<R2Settings>()
            .Bind(config.GetSection(R2Settings.SectionName))
            .Validate(s => !string.IsNullOrWhiteSpace(s.BucketName), "R2: BucketName missing")
            .Validate(s => !string.IsNullOrWhiteSpace(s.PublicBaseUrl), "R2: PublicBaseUrl missing")
            .ValidateOnStart();

        return services;
    }
}
