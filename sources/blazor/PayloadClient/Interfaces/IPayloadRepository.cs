namespace PayloadClient.Interfaces;

public interface IPayloadRepository<T> where T : class
{
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(string id, T entity);
    Task<bool> DeleteAsync(string id);
}