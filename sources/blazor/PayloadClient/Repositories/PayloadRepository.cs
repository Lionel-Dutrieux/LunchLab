using PayloadClient.Interfaces;
using PayloadClient.Models;
using Microsoft.Extensions.Logging;
using PayloadClient.Exceptions;
using PayloadClient.Query;

namespace PayloadClient.Repositories;

public class PayloadRepository<T> : BasePayloadRepository, IPayloadRepository<T> where T : class
{
    private readonly ILogger<PayloadRepository<T>> _logger;

    public PayloadRepository(
        IHttpClientFactory httpClientFactory, 
        string endpoint,
        ILogger<PayloadRepository<T>> logger) 
        : base(httpClientFactory, endpoint, logger)
    {
        _logger = logger;
    }

    public virtual async Task<T?> GetByIdAsync(string id, string? jwtToken = null)
    {
        var response = await GetAsync<PayloadResponse<T>>($"{_endpoint}/{id}", jwtToken);
        return response?.Doc;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(string? jwtToken = null)
    {
        var query = new PayloadQueryBuilder().WithDepth(1);
        var response = await GetWithQueryAsync<PayloadResponse<T>>(query, jwtToken);
        return response?.Docs ?? Enumerable.Empty<T>();
    }

    public virtual async Task<T> CreateAsync<TData>(TData data, string? jwtToken = null)
    {
        var response = await PostAsync<TData, PayloadResponse<T>>(_endpoint, data, jwtToken);
        return response?.Doc ?? throw new InvalidOperationException("Failed to create entity");
    }

    public virtual async Task<T> UpdateAsync<TData>(string id, TData data, string? jwtToken = null)
    {
        var response = await PatchAsync<TData, PayloadResponse<T>>($"{_endpoint}/{id}", data, jwtToken);
        return response?.Doc ?? throw new InvalidOperationException("Failed to update entity");
    }

    public virtual async Task<bool> DeleteAsync(string id, string? jwtToken = null)
    {
        return await DeleteAsync($"{_endpoint}/{id}", jwtToken);
    }
}