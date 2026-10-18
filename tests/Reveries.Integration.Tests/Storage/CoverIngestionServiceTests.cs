using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging.Abstractions;
using Reveries.Domain.Editions;
using Reveries.Integration.Storage;
using SkiaSharp;

namespace Reveries.Integration.Tests.Storage;

public class CoverIngestionServiceTests
{
    private const string SourceUrl = "https://external.example/cover.jpg";

    [Fact]
    public async Task Stores_three_variants_and_returns_detail_and_thumb_urls()
    {
        var (sut, store) = Build(ImageResponse(PngBytes()));

        var result = await sut.IngestAsync(Cover.TryCreate(SourceUrl, null), Isbn.Create("9780132350884"), CancellationToken.None);

        Assert.NotNull(result);
        Assert.EndsWith("/detail.webp", result!.Url);
        Assert.EndsWith("/thumb.webp", result.ThumbnailUrl);

        Assert.Equal(3, store.Uploads.Count);
        Assert.Contains(store.Uploads, u => u.Key.EndsWith("/original.png") && u.ContentType == "image/png");
        Assert.Contains(store.Uploads, u => u.Key.EndsWith("/detail.webp") && u.ContentType == "image/webp");
        Assert.Contains(store.Uploads, u => u.Key.EndsWith("/thumb.webp") && u.ContentType == "image/webp");
    }

    [Fact]
    public async Task Writes_provenance_and_cache_metadata_on_each_variant()
    {
        var (sut, store) = Build(ImageResponse(PngBytes()));

        await sut.IngestAsync(Cover.TryCreate(SourceUrl, null), Isbn.Create("9780132350884"), CancellationToken.None);

        var detail = store.Uploads.Single(u => u.Key.EndsWith("/detail.webp"));
        Assert.Equal("detail", detail.Metadata["variant"]);
        Assert.Equal("9780132350884", detail.Metadata["isbn"]);
        Assert.Equal(SourceUrl, detail.Metadata["source-url"]);
        Assert.Equal("public, max-age=31536000, immutable", detail.CacheControl);
    }

    [Fact]
    public async Task Keys_share_the_content_hash_prefix()
    {
        var (sut, store) = Build(ImageResponse(PngBytes()));

        await sut.IngestAsync(Cover.TryCreate(SourceUrl, null), null, CancellationToken.None);

        var prefixes = store.Uploads.Select(u => u.Key[..u.Key.IndexOf('/')]).Distinct();
        Assert.Single(prefixes);
    }

    [Fact]
    public async Task Skips_upload_when_the_cover_already_exists()
    {
        var (sut, store) = Build(ImageResponse(PngBytes()), exists: true);

        var result = await sut.IngestAsync(Cover.TryCreate(SourceUrl, null), null, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(store.Uploads);
    }

    [Fact]
    public async Task Returns_null_for_a_null_source()
    {
        var (sut, store) = Build(ImageResponse(PngBytes()));

        Assert.Null(await sut.IngestAsync(null, null, CancellationToken.None));
        Assert.Empty(store.Uploads);
    }

    [Fact]
    public async Task Returns_null_when_the_image_cannot_be_decoded()
    {
        var (sut, store) = Build(ImageResponse([1, 2, 3, 4]));

        Assert.Null(await sut.IngestAsync(Cover.TryCreate(SourceUrl, null), null, CancellationToken.None));
        Assert.Empty(store.Uploads);
    }

    [Fact]
    public async Task Returns_null_for_a_non_image_content_type()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("<html/>") };
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
        var (sut, store) = Build(response);

        Assert.Null(await sut.IngestAsync(Cover.TryCreate(SourceUrl, null), null, CancellationToken.None));
        Assert.Empty(store.Uploads);
    }

    [Fact]
    public async Task Returns_null_on_an_http_error()
    {
        var (sut, store) = Build(new HttpResponseMessage(HttpStatusCode.NotFound));

        Assert.Null(await sut.IngestAsync(Cover.TryCreate(SourceUrl, null), null, CancellationToken.None));
        Assert.Empty(store.Uploads);
    }

    [Fact]
    public async Task Returns_null_when_the_image_is_over_the_size_limit()
    {
        var (sut, store) = Build(ImageResponse(new byte[11 * 1024 * 1024]));

        Assert.Null(await sut.IngestAsync(Cover.TryCreate(SourceUrl, null), null, CancellationToken.None));
        Assert.Empty(store.Uploads);
    }

    private static (CoverIngestionService Sut, FakeObjectStore Store) Build(HttpResponseMessage response, bool exists = false)
    {
        var httpClient = new HttpClient(new StubHttpMessageHandler(response));
        var store = new FakeObjectStore { Exists = exists };
        var sut = new CoverIngestionService(httpClient, store, NullLogger<CoverIngestionService>.Instance);
        return (sut, store);
    }

    private static HttpResponseMessage ImageResponse(byte[] bytes, string contentType = "image/png")
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(bytes) };
        response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return response;
    }

    private static byte[] PngBytes(int width = 400, int height = 600)
    {
        using var bitmap = new SKBitmap(width, height);
        using (var canvas = new SKCanvas(bitmap))
            canvas.Clear(SKColors.CornflowerBlue);

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public StubHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }

    private sealed class FakeObjectStore : IObjectStore
    {
        public bool Exists { get; init; }
        public List<ObjectUpload> Uploads { get; } = [];

        public Task<bool> ExistsAsync(string key, CancellationToken ct)
        {
            return Task.FromResult(Exists);
        }

        public Task PutAsync(ObjectUpload upload, CancellationToken ct)
        {
            Uploads.Add(upload);
            return Task.CompletedTask;
        }

        public string PublicUrl(string key)
        {
            return $"https://cdn.test/{key}";
        }
    }
}
