using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Reveries.Application.Common.Exceptions;
using Reveries.Integration.Common.Helpers;
using DecimalConverter = Reveries.Integration.Common.Helpers.DecimalConverter;

namespace Reveries.Integration.Common.Base;

public abstract class ExternalBaseClient<TClient> where TClient : class
{
    protected readonly HttpClient HttpClient;
    private readonly ILogger<TClient> _logger;
    protected abstract string DependencyName { get; }
    
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new DecimalConverter() }
    };
    
    protected ExternalBaseClient(HttpClient httpClient, ILogger<TClient> logger)
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
                _logger.LogWarning("{Dependency} key is invalid or expired", DependencyName);
                return null;
            case HttpStatusCode.NotFound:
                _logger.LogInformation("{Dependency} returned 404 for '{Context}'", DependencyName ,context);
                return null;
            case HttpStatusCode.TooManyRequests:
                _logger.LogWarning("{Dependency} rate limit exceeded for '{Context}'", DependencyName, context);
                return null;
        }

        if (!response.IsSuccessStatusCode)
            throw new ExternalDependencyException(
                dependency: DependencyName,
                message: $"Upstream returned {(int)response.StatusCode} ({response.StatusCode}) for {context}",
                upstreamStatus: response.StatusCode);

        var json = await response.Content.ReadAsStringAsync(ct);

        try
        {
            var result = JsonSerializer.Deserialize<T>(json, _jsonOptions);

            if (validate is not null && !validate(result))
            {
                _logger.LogWarning("{Dependency} returned empty response for {Context}", 
                    DependencyName, 
                    context);
                return null;
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize {Dependency} response for '{Context}'. Payload: {Payload}",
                DependencyName,
                context, 
                json.TruncateForLog());
            throw new InvalidOperationException($"Failed to deserialize {DependencyName} response for '{context}'.", ex);
        }
    }
}