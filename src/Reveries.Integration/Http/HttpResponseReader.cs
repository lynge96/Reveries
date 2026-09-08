using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Logging;
using Reveries.Application.Common.Exceptions;
using Reveries.Integration.Http.Helpers;

namespace Reveries.Integration.Http;

public static class HttpResponseReader
{
    public static async Task<T?> ReadAsync<T>(
        HttpResponseMessage response,
        JsonTypeInfo<T> typeInfo,
        string dependency,
        string context,
        ILogger logger,
        Func<T?, bool>? validate = null,
        CancellationToken ct = default) where T : class
    {
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            logger.LogInformation("{Dependency} returned 404 for '{Context}'", dependency, context);
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            var status = (int)response.StatusCode;
            logger.LogWarning("{Dependency} returned {Status} ({StatusName}) for '{Context}'",
                dependency, status, response.StatusCode, context);

            throw new ExternalDependencyException(
                dependency: dependency,
                message: $"Upstream returned {status} ({response.StatusCode}) for {context}",
                upstreamStatus: status);
        }

        var json = await response.Content.ReadAsStringAsync(ct);

        try
        {
            var result = JsonSerializer.Deserialize(json, typeInfo);

            if (validate is not null && !validate(result))
            {
                logger.LogWarning("{Dependency} returned an empty response for '{Context}'", dependency, context);
                return null;
            }

            return result;
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Failed to deserialize {Dependency} response for '{Context}'. Payload: {Payload}",
                dependency, context, json.TruncateForLog());

            throw new ExternalDependencyException(
                dependency: dependency,
                message: $"Failed to deserialize {dependency} response for '{context}'.",
                innerException: ex);
        }
    }
}