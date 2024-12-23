using PayloadClient.Interfaces;
using PayloadClient.Models;
using Microsoft.Extensions.Logging;
using PayloadClient.Exceptions;

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

    public async Task<T?> GetByIdAsync(string id)
    {
        try
        {
            var response = await GetAsync<PayloadResponse<T>>($"{_endpoint}/{id}");
            if (response?.Doc == null)
            {
                throw new PayloadNotFoundException($"Entity of type {typeof(T).Name} with ID {id} not found");
            }
            return response.Doc;
        }
        catch (PayloadException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting {Type} with ID {Id}", typeof(T).Name, id);
            throw new PayloadException($"Failed to get {typeof(T).Name}", ex);
        }
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