namespace Reveries.Integration.Storage.Configuration;

public sealed class R2Settings
{
    public const string SectionName = "R2";

    public required string BucketName { get; init; }
    public required string PublicBaseUrl { get; init; }
    public string? AccountId { get; init; }
    public string? AccessKeyId { get; init; }
    public string? SecretAccessKey { get; init; }
}
