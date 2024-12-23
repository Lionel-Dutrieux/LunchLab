using PayloadClient.Interfaces;
using PayloadClient.Models;

namespace PayloadClient.Repositories;

public class PayloadRepository<T> : BasePayloadRepository, IPayloadRepository<T> where T : class
{
    public PayloadRepository(IHttpClientFactory httpClientFactory, string endpoint) 
        : base(httpClientFactory, endpoint)
    {
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        var response = await GetAsync<PayloadResponse<T>>($"{_endpoint}/{id}");
        return response?.Doc;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var response = await GetAsync<PayloadResponse<T>>(_endpoint);
        return response?.Docs ?? Enumerable.Empty<T>();
    }

    public async Task<T> CreateAsync(T entity)
    {
        var response = await PostAsync<T, PayloadResponse<T>>(_endpoint, entity);
        return response?.Doc ?? throw new InvalidOperationException("Failed to create entity");
    }

    public async Task<T> UpdateAsync(string id, T entity)
    {
        var response = await PatchAsync<T, PayloadResponse<T>>($"{_endpoint}/{id}", entity);
        return response?.Doc ?? throw new InvalidOperationException("Failed to update entity");
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await DeleteAsync($"{_endpoint}/{id}");
    }
}