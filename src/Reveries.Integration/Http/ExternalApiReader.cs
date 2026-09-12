using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Logging;
using Reveries.Application.Common.Exceptions;

namespace Reveries.Integration.Http;

public sealed class ExternalApiReader
{
    private readonly string _dependency;
    private readonly ILogger _logger;

    public ExternalApiReader(string dependency, ILogger logger)
    {
        _dependency = dependency;
        _logger = logger;
    }

    public async Task<T?> ReadAsync<T>(
        HttpResponseMessage response,
        JsonTypeInfo<T> typeInfo,
        string context,
        CancellationToken ct = default) where T : class
    {
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogDebug("{Dependency} returned 404 for '{Context}'", _dependency, context);
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            var status = (int)response.StatusCode;
            _logger.LogDebug("{Dependency} returned {Status} ({StatusName}) for '{Context}'",
                _dependency, status, response.StatusCode, context);

            throw new ExternalDependencyException(
                dependency: _dependency,
                message: $"Upstream returned {status} ({response.StatusCode}) for {context}",
                upstreamStatus: status);
        }

        var json = await response.Content.ReadAsStringAsync(ct);

        try
        {
            return JsonSerializer.Deserialize(json, typeInfo);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize {Dependency} response for '{Context}'. Payload: {Payload}",
                _dependency, context, Truncate(json));

            throw new ExternalDependencyException(
                dependency: _dependency,
                message: $"Failed to deserialize {_dependency} response for '{context}'.",
                innerException: ex);
        }
    }

    private static string Truncate(string value, int maxLength = 1000)
    {
        return value.Length <= maxLength ? value : value[..maxLength] + "…";
    }
}