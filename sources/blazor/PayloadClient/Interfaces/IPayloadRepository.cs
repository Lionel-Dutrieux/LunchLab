namespace PayloadClient.Interfaces;

public interface IPayloadRepository<T> where T : class
{
    Task<T?> GetByIdAsync(string id, string? jwtToken = null);
    Task<IEnumerable<T>> GetAllAsync(string? jwtToken = null);
    Task<T> CreateAsync<TData>(TData data, string? jwtToken = null);
    Task<T> UpdateAsync<TData>(string id, TData data, string? jwtToken = null);
    Task<bool> DeleteAsync(string id, string? jwtToken = null);
}