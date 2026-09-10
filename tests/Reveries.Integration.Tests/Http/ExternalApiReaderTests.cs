using System.Net;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging.Abstractions;
using Reveries.Application.Common.Exceptions;
using Reveries.Integration.Http;

namespace Reveries.Integration.Tests.Http;

public class ExternalApiReaderTests
{
    private static ExternalApiReader Reader() => new("TestApi", NullLogger.Instance);

    [Fact]
    public async Task ReadAsync_NotFound_ReturnsNull()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.NotFound);

        var result = await Reader().ReadAsync(response, ReaderTestJsonContext.Default.ReaderTestDto, "ctx");

        Assert.Null(result);
    }

    [Fact]
    public async Task ReadAsync_ServerError_ThrowsWithUpstreamStatus()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        var ex = await Assert.ThrowsAsync<ExternalDependencyException>(
            () => Reader().ReadAsync(response, ReaderTestJsonContext.Default.ReaderTestDto, "ctx"));

        Assert.Equal(500, ex.UpstreamStatus);
    }

    [Fact]
    public async Task ReadAsync_Success_Deserializes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{"Name":"hello"}"""),
        };

        var result = await Reader().ReadAsync(response, ReaderTestJsonContext.Default.ReaderTestDto, "ctx");

        Assert.Equal("hello", result?.Name);
    }

    [Fact]
    public async Task ReadAsync_MalformedJson_ThrowsExternalDependency()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("not json at all"),
        };

        await Assert.ThrowsAsync<ExternalDependencyException>(
            () => Reader().ReadAsync(response, ReaderTestJsonContext.Default.ReaderTestDto, "ctx"));
    }
}

internal sealed record ReaderTestDto
{
    public string? Name { get; init; }
}

[JsonSerializable(typeof(ReaderTestDto))]
internal sealed partial class ReaderTestJsonContext : JsonSerializerContext;