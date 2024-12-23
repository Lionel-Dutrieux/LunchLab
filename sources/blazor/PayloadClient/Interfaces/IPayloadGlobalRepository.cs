namespace PayloadClient.Interfaces;

public interface IPayloadGlobalRepository<T> where T : class
{
    Task<T?> GetGlobalAsync();
    Task<T> UpdateGlobalAsync(T entity);
}