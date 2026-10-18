using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reveries.Application.Books.Interfaces;

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

        services.AddSingleton<IAmazonS3>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<R2Settings>>().Value;

            var s3Config = new AmazonS3Config
            {
                ServiceURL = $"https://{settings.AccountId}.r2.cloudflarestorage.com",
                ForcePathStyle = true,
                AuthenticationRegion = "auto",
                RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
            };

            return new AmazonS3Client(settings.AccessKeyId, settings.SecretAccessKey, s3Config);
        });

        services.AddSingleton<IObjectStore, R2ObjectStore>();
        services.AddHttpClient<ICoverImageStore, CoverIngestionService>();

        return services;
    }
}
