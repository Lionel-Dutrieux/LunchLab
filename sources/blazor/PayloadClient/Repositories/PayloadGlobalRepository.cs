using Microsoft.Extensions.Logging;
using PayloadClient.Interfaces;
using PayloadClient.Models;

namespace PayloadClient.Repositories;

public class PayloadGlobalRepository<T> : BasePayloadRepository, IPayloadGlobalRepository<T> where T : class
{
    public PayloadGlobalRepository(
        IHttpClientFactory httpClientFactory,
        string endpoint,
        ILogger<PayloadGlobalRepository<T>> logger) 
        : base(httpClientFactory, $"globals/{endpoint}", logger)
    {
    }

    public async Task<T?> GetBySlugAsync(string slug, string? jwtToken = null)
    {
        var response = await GetAsync<PayloadResponse<T>>($"{_endpoint}/{slug}", jwtToken);
        return response?.Doc;
    }

    public async Task<IEnumerable<T>> GetAllAsync(string? jwtToken = null)
    {
        var response = await GetAsync<PayloadResponse<T>>(_endpoint, jwtToken);
        return response?.Docs ?? Enumerable.Empty<T>();
    }
}