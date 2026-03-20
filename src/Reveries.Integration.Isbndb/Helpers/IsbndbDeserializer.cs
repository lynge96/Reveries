using System.Text.Json;
using Microsoft.Extensions.Logging;
using Reveries.Application.Extensions;
using Reveries.Integration.Isbndb.Mappers.Converters;

namespace Reveries.Integration.Isbndb.Helpers;

public static class IsbndbDeserializer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new DecimalConverter() }
    };
    
    public static async Task<T?> DeserializeAsync<T>(
        HttpResponseMessage response,
        string context,
        ILogger logger,
        Func<T?, bool>? validate = null,
        CancellationToken ct = default)
        where T : class
    {
        var json = await response.Content.ReadAsStringAsync(ct);

        try
        {
            var result = JsonSerializer.Deserialize<T>(json, JsonOptions);

            if (validate is not null && !validate(result))
            {
                logger.LogWarning("ISBNdb returned empty response for {Context}", context);
                return null;
            }

            return result;
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Failed to deserialize ISBNdb response for {Context}. Payload: {Payload}",
                context, json.TruncateForLog());
            throw new InvalidOperationException($"Failed to deserialize ISBNdb response for {context}.", ex);
        }
    }
}