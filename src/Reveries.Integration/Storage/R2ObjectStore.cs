using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Reveries.Integration.Storage.Configuration;

namespace Reveries.Integration.Storage;

public sealed class R2ObjectStore : IObjectStore
{
    private readonly IAmazonS3 _s3;
    private readonly R2Settings _settings;

    public R2ObjectStore(IAmazonS3 s3, IOptions<R2Settings> settings)
    {
        _s3 = s3;
        _settings = settings.Value;
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken ct)
    {
        try
        {
            await _s3.GetObjectMetadataAsync(_settings.BucketName, key, ct);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task PutAsync(ObjectUpload upload, CancellationToken ct)
    {
        using var stream = new MemoryStream(upload.Content, writable: false);

        // R2 rejects AWS SDK v4's default streaming payload signing
        // (STREAMING-AWS4-HMAC-SHA256-PAYLOAD); UNSIGNED-PAYLOAD is used instead over HTTPS.
        var request = new PutObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = upload.Key,
            InputStream = stream,
            ContentType = upload.ContentType,
            DisablePayloadSigning = true
        };
        request.Headers.CacheControl = upload.CacheControl;
        foreach (var (name, value) in upload.Metadata)
            request.Metadata.Add(name, value);

        await _s3.PutObjectAsync(request, ct);
    }

    public string PublicUrl(string key)
    {
        return $"{_settings.PublicBaseUrl.TrimEnd('/')}/{key}";
    }
}
