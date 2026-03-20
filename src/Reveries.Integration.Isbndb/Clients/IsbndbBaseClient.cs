using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Reveries.Application.Exceptions;
using Reveries.Integration.Isbndb.Helpers;

namespace Reveries.Integration.Isbndb.Clients;

public abstract class IsbndbBaseClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    
    protected abstract string DependencyName { get; }

    protected IsbndbBaseClient(HttpClient httpClient, ILogger logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    protected async Task<T?> GetAsync<T>(
        string endpoint, 
        string context, 
        Func<T?, bool>? validate = null, 
        CancellationToken ct = default) where T : class
    {
        var response = await _httpClient.GetAsync(endpoint, ct);
        
        return await HandleResponseAsync(response, context, validate, ct);
    }

    protected async Task<T?> PostAsync<T>(
        string endpoint, 
        object body, 
        string context, 
        Func<T?, bool>? validate = null, 
        CancellationToken ct = default) where T : class
    {
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(endpoint, content, ct);
        
        return await HandleResponseAsync(response, context, validate: validate, ct: ct);
    }
    
    private async Task<T?> HandleResponseAsync<T>(
        HttpResponseMessage response, 
        string context, 
        Func<T?, bool>? validate = null, 
        CancellationToken ct = default)
        where T : class
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden:
                _logger.LogError("ISBNdb API key is invalid or expired");
                return null;
            case HttpStatusCode.NotFound:
                _logger.LogDebug("ISBNdb returned 404 for {Context}", context);
                return null;
        }

        if (!response.IsSuccessStatusCode)
            throw new ExternalDependencyException(
                dependency: DependencyName,
                message: $"ISBNdb returned {(int)response.StatusCode} ({response.StatusCode}) for {context}",
                upstreamStatus: response.StatusCode);

        return await IsbndbDeserializer.DeserializeAsync(
            response,
            context: context,
            logger: _logger,
            validate: validate,
            ct: ct);
    }
}