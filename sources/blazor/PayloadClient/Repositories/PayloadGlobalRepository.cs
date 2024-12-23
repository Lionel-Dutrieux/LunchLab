using PayloadClient.Interfaces;
using PayloadClient.Models;

namespace PayloadClient.Repositories;

public class PayloadGlobalRepository<T> : BasePayloadRepository, IPayloadGlobalRepository<T> where T : class
{
    public PayloadGlobalRepository(IHttpClientFactory httpClientFactory, string endpoint) 
        : base(httpClientFactory, endpoint)
    {
    }

    public async Task<T?> GetGlobalAsync()
    {
        var response = await GetAsync<PayloadResponse<T>>(_endpoint);
        return response?.Doc;
    }

    public async Task<T> UpdateGlobalAsync(T entity)
    {
        var response = await PatchAsync<T, PayloadResponse<T>>(_endpoint, entity);
        return response?.Doc ?? throw new InvalidOperationException("Failed to update global");
    }
}