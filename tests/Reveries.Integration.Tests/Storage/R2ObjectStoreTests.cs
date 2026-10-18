using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Reveries.Integration.Storage;
using Reveries.Integration.Storage.Configuration;

namespace Reveries.Integration.Tests.Storage;

public class R2ObjectStoreTests
{
    [Fact]
    public async Task PutAsync_disables_payload_signing_and_sets_headers_and_metadata()
    {
        var s3 = Substitute.For<IAmazonS3>();
        PutObjectRequest? captured = null;
        s3.PutObjectAsync(Arg.Do<PutObjectRequest>(r => captured = r), Arg.Any<CancellationToken>())
            .Returns(new PutObjectResponse());

        var upload = new ObjectUpload(
            "abc/detail.webp",
            [1, 2, 3],
            "image/webp",
            "public, max-age=31536000, immutable",
            new Dictionary<string, string> { ["variant"] = "detail", ["isbn"] = "9780132350884" });

        await Build(s3).PutAsync(upload, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.True(captured!.DisablePayloadSigning is true);
        Assert.Equal("reveries-covers", captured.BucketName);
        Assert.Equal("abc/detail.webp", captured.Key);
        Assert.Equal("image/webp", captured.ContentType);
        Assert.Equal("public, max-age=31536000, immutable", captured.Headers.CacheControl);
        Assert.Equal("detail", captured.Metadata["variant"]);
        Assert.Equal("9780132350884", captured.Metadata["isbn"]);
    }

    [Fact]
    public async Task ExistsAsync_returns_false_when_the_object_is_not_found()
    {
        var s3 = Substitute.For<IAmazonS3>();
        s3.GetObjectMetadataAsync("reveries-covers", "missing", Arg.Any<CancellationToken>())
            .ThrowsAsync(new AmazonS3Exception("not found") { StatusCode = HttpStatusCode.NotFound });

        Assert.False(await Build(s3).ExistsAsync("missing", CancellationToken.None));
    }

    [Fact]
    public async Task ExistsAsync_returns_true_when_the_object_exists()
    {
        var s3 = Substitute.For<IAmazonS3>();
        s3.GetObjectMetadataAsync("reveries-covers", "found", Arg.Any<CancellationToken>())
            .Returns(new GetObjectMetadataResponse());

        Assert.True(await Build(s3).ExistsAsync("found", CancellationToken.None));
    }

    [Fact]
    public void PublicUrl_joins_the_base_url_and_key_without_a_double_slash()
    {
        var s3 = Substitute.For<IAmazonS3>();

        Assert.Equal("https://covers.reveries.dk/abc/detail.webp", Build(s3).PublicUrl("abc/detail.webp"));
    }

    private static R2ObjectStore Build(IAmazonS3 s3)
    {
        var settings = Options.Create(new R2Settings
        {
            BucketName = "reveries-covers",
            PublicBaseUrl = "https://covers.reveries.dk",
            AccountId = "acc",
            AccessKeyId = "key",
            SecretAccessKey = "secret"
        });

        return new R2ObjectStore(s3, settings);
    }
}
