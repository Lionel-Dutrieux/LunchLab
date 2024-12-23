using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PayloadClient.Exceptions;
using PayloadClient.Query;

namespace PayloadClient.Repositories;

public abstract class BasePayloadRepository
{
    protected readonly HttpClient _httpClient;
    protected readonly string _endpoint;
    private readonly ILogger<BasePayloadRepository> _logger;

    protected BasePayloadRepository(
        IHttpClientFactory httpClientFactory, 
        string endpoint,
        ILogger<BasePayloadRepository> logger)
    {
        _httpClient = httpClientFactory.CreateClient("PayloadClient");
        _endpoint = endpoint;
        _logger = logger;
    }

    protected async Task<TResponse?> GetAsync<TResponse>(string url)
    {
        try
        {
            var apiUrl = $"api/{url}";
            var response = await _httpClient.GetAsync(apiUrl);
            
            await EnsureSuccessStatusCodeWithBetterErrorAsync(response);
            
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch (JsonException ex)
            {
                throw new PayloadDeserializationException(
                    $"Failed to deserialize response from {apiUrl}",
                    content,
                    typeof(TResponse),
                    ex);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for {Url}", url);
            throw new PayloadException($"Failed to complete request to {url}", ex);
        }
    }

    protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
    {
        try
        {
            var apiUrl = $"api/{url}";
            var response = await _httpClient.PostAsJsonAsync(apiUrl, data);
            
            await EnsureSuccessStatusCodeWithBetterErrorAsync(response);
            
            var content = await response.Content.ReadAsStringAsync();
            try 
            {
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch (JsonException ex)
            {
                throw new PayloadDeserializationException(
                    $"Failed to deserialize response from {apiUrl}",
                    content,
                    typeof(TResponse),
                    ex);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for {Url} with data {Data}", url, data);
            throw new PayloadException($"Failed to complete request to {url}", ex);
        }
    }

    protected async Task<TResponse?> PatchAsync<TRequest, TResponse>(string url, TRequest data)
    {
        var apiUrl = $"api/{url}";
        var response = await _httpClient.PatchAsJsonAsync(apiUrl, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    protected async Task<bool> DeleteAsync(string url)
    {
        var apiUrl = $"api/{url}";
        var response = await _httpClient.DeleteAsync(apiUrl);
        return response.IsSuccessStatusCode;
    }

    protected string BuildUrl(PayloadQueryBuilder? queryBuilder = null)
    {
        var url = _endpoint;
        if (queryBuilder != null)
        {
            url += queryBuilder.Build();
        }
        return url;
    }

    protected async Task<TResponse?> GetWithQueryAsync<TResponse>(PayloadQueryBuilder queryBuilder)
    {
        return await GetAsync<TResponse>(BuildUrl(queryBuilder));
    }

    private async Task EnsureSuccessStatusCodeWithBetterErrorAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;

        var content = await response.Content.ReadAsStringAsync();
        
        switch (response.StatusCode)
        {
            case HttpStatusCode.NotFound:
                throw new PayloadNotFoundException($"Resource not found: {response.RequestMessage?.RequestUri}");
            
            case HttpStatusCode.BadRequest:
                throw new PayloadException($"Bad request: {content}", response.StatusCode);
                
            case HttpStatusCode.Unauthorized:
                throw new PayloadException("Unauthorized access", response.StatusCode);
                
            default:
                throw new PayloadException(
                    $"API request failed with status code {response.StatusCode}: {content}", 
                    response.StatusCode);
        }
    }
}