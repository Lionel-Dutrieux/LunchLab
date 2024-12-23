using System.Net.Http.Json;

namespace PayloadClient.Repositories;
using PayloadClient.Query;

public abstract class BasePayloadRepository
{
    protected readonly HttpClient _httpClient;
    protected readonly string _endpoint;

    protected BasePayloadRepository(IHttpClientFactory httpClientFactory, string endpoint)
    {
        _httpClient = httpClientFactory.CreateClient("PayloadClient");
        _endpoint = endpoint;
    }

    protected async Task<TResponse?> GetAsync<TResponse>(string url)
    {
        var apiUrl = $"api/{url}";
        var response = await _httpClient.GetAsync(apiUrl);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
    {
        var apiUrl = $"api/{url}";
        var response = await _httpClient.PostAsJsonAsync(apiUrl, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>();
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
}