using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using SkiaSharp;

namespace Reveries.Integration.Storage;

public sealed class CoverIngestionService : ICoverImageStore
{
    private const int DetailWidth = 1200;
    private const int ThumbWidth = 180;
    private const int WebpQuality = 80;
    private const string CacheControl = "public, max-age=31536000, immutable";

    private readonly HttpClient _httpClient;
    private readonly IObjectStore _objectStore;
    private readonly ILogger<CoverIngestionService> _logger;

    public CoverIngestionService(HttpClient httpClient, IObjectStore objectStore, ILogger<CoverIngestionService> logger)
    {
        _httpClient = httpClient;
        _objectStore = objectStore;
        _logger = logger;
    }

    public async Task<StoredCover?> IngestAsync(Cover? source, Isbn? isbn, CancellationToken ct)
    {
        if (source is null)
            return null;

        var bytes = await _httpClient.GetByteArrayAsync(source.Url, ct);
        var sha = Convert.ToHexStringLower(SHA256.HashData(bytes));

        var detailKey = $"{sha}/detail.webp";
        var thumbKey = $"{sha}/thumb.webp";

        if (await _objectStore.ExistsAsync(detailKey, ct))
        {
            _logger.LogDebug("Cover {Sha} already stored; reusing existing objects.", sha);
            return new StoredCover(_objectStore.PublicUrl(detailKey), _objectStore.PublicUrl(thumbKey));
        }

        using var decoded = SKBitmap.Decode(bytes);
        if (decoded is null)
        {
            _logger.LogWarning("Could not decode cover image from {Url}; saving the book without a stored cover.", source.Url);
            return null;
        }

        var (contentType, extension) = DetectOriginal(bytes);
        var provenance = Provenance(source, isbn);

        // Resize synchronously (reads the shared bitmap), then upload the three variants in parallel.
        var originalUpload = new ObjectUpload($"{sha}/original.{extension}", bytes, contentType, CacheControl, Metadata(provenance, "original"));
        var detailUpload = new ObjectUpload(detailKey, EncodeWebp(decoded, DetailWidth), "image/webp", CacheControl, Metadata(provenance, "detail"));
        var thumbUpload = new ObjectUpload(thumbKey, EncodeWebp(decoded, ThumbWidth), "image/webp", CacheControl, Metadata(provenance, "thumb"));

        await Task.WhenAll(
            _objectStore.PutAsync(originalUpload, ct),
            _objectStore.PutAsync(detailUpload, ct),
            _objectStore.PutAsync(thumbUpload, ct));

        _logger.LogInformation("Stored cover {Sha} ({DetailWidth}px + {ThumbWidth}px).", sha, DetailWidth, ThumbWidth);

        return new StoredCover(_objectStore.PublicUrl(detailKey), _objectStore.PublicUrl(thumbKey));
    }

    private static Dictionary<string, string> Provenance(Cover source, Isbn? isbn)
    {
        var metadata = new Dictionary<string, string> { ["source-url"] = source.Url };

        var isbnValue = isbn?.Value13 ?? isbn?.Value10;
        if (isbnValue is not null)
            metadata["isbn"] = isbnValue;

        return metadata;
    }

    private static Dictionary<string, string> Metadata(IReadOnlyDictionary<string, string> provenance, string variant)
    {
        return new Dictionary<string, string>(provenance) { ["variant"] = variant };
    }

    private static byte[] EncodeWebp(SKBitmap source, int maxWidth)
    {
        SKBitmap? scaled = null;
        try
        {
            var bitmap = source;

            if (source.Width > maxWidth)
            {
                var height = (int)Math.Round(source.Height * (maxWidth / (double)source.Width));
                scaled = source.Resize(new SKImageInfo(maxWidth, height), SKFilterQuality.High);
                bitmap = scaled ?? source;
            }

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Webp, WebpQuality);
            return data.ToArray();
        }
        finally
        {
            scaled?.Dispose();
        }
    }

    private static (string ContentType, string Extension) DetectOriginal(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes, writable: false);
        using var codec = SKCodec.Create(stream);

        return codec?.EncodedFormat switch
        {
            SKEncodedImageFormat.Jpeg => ("image/jpeg", "jpg"),
            SKEncodedImageFormat.Png => ("image/png", "png"),
            SKEncodedImageFormat.Webp => ("image/webp", "webp"),
            SKEncodedImageFormat.Gif => ("image/gif", "gif"),
            SKEncodedImageFormat.Bmp => ("image/bmp", "bmp"),
            _ => ("application/octet-stream", "bin")
        };
    }
}
