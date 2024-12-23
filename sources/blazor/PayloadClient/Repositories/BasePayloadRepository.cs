using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PayloadClient.Exceptions;
using PayloadClient.Query;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using PayloadClient.Converters;
using PayloadClient.Models;

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

    protected async Task<TResponse?> GetAsync<TResponse>(string url, string? jwtToken = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/{url}");
        if (!string.IsNullOrEmpty(jwtToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }
        return await SendRequestAsync<TResponse>(request);
    }

    protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data, string? jwtToken = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/{url}")
        {
            Content = JsonContent.Create(data)
        };
        if (!string.IsNullOrEmpty(jwtToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }
        return await SendRequestAsync<TResponse>(request);
    }

    protected async Task<TResponse?> PatchAsync<TRequest, TResponse>(string url, TRequest data, string? jwtToken = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch, $"api/{url}")
        {
            Content = JsonContent.Create(data)
        };
        if (!string.IsNullOrEmpty(jwtToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }
        return await SendRequestAsync<TResponse>(request);
    }

    protected async Task<bool> DeleteAsync(string url, string? jwtToken = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/{url}");
        if (!string.IsNullOrEmpty(jwtToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }
        var response = await _httpClient.SendAsync(request);
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

    protected async Task<TResponse?> GetWithQueryAsync<TResponse>(PayloadQueryBuilder queryBuilder, string? jwtToken = null)
    {
        var url = $"api/{_endpoint}{queryBuilder.Build()}";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (!string.IsNullOrEmpty(jwtToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }
        return await SendRequestAsync<TResponse>(request);
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

    private async Task<TResponse?> SendRequestAsync<TResponse>(HttpRequestMessage request)
    {
        var apiUrl = request.RequestUri?.ToString() ?? string.Empty;
        if (!apiUrl.StartsWith("api/"))
        {
            request.RequestUri = new Uri($"api/{apiUrl}", UriKind.Relative);
        }
        
        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        _logger.LogDebug("Response content: {Content}", content);
        
        await EnsureSuccessStatusCodeWithBetterErrorAsync(response);
        
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }
}