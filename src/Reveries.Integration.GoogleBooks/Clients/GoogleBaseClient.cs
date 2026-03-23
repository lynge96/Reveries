using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Application.Extensions;

namespace Reveries.Integration.GoogleBooks.Clients;

public abstract class GoogleBaseClient<TClient> where TClient : class
{
    protected readonly HttpClient HttpClient;
    private readonly ILogger<TClient> _logger;
    
    protected abstract string DependencyName { get; }
    
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    protected GoogleBaseClient(HttpClient httpClient, ILogger<TClient> logger)
    {
        HttpClient = httpClient;
        _logger = logger;
    }

    protected async Task<T?> HandleResponseAsync<T>(
        HttpResponseMessage response,
        string context,
        Func<T?, bool>? validate = null,
        CancellationToken ct = default) where T : class
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized:
                _logger.LogError("Google Books API key is invalid or expired");
                return null;
            case HttpStatusCode.NotFound:
                _logger.LogDebug("Google Books returned 404 for '{Context}'", context);
                return null;
            case HttpStatusCode.TooManyRequests:
                _logger.LogWarning("Google Books API rate limit exceeded for '{Context}'", context);
                return null;
        }
        
        if (!response.IsSuccessStatusCode)
        {
            throw new ExternalDependencyException(
                dependency: DependencyName,
                message: $"Google Books returned {(int)response.StatusCode} ({response.StatusCode}) for {context}",
                upstreamStatus: response.StatusCode
            );
        }
        
        var json = await response.Content.ReadAsStringAsync(ct);
        
        try
        {
            var result = JsonSerializer.Deserialize<T>(json, _jsonOptions);
            
            if (validate is not null && !validate(result))
            {
                _logger.LogWarning("Google Books returned empty response for {Context}", context);
                return null;
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize Google Books response for '{Context}'. Payload: {Payload}", 
                context, 
                json.TruncateForLog());
            throw new InvalidOperationException($"Failed to deserialize Google Books response for '{context}'.", ex);
        }
    }
}