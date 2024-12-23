namespace PayloadClient.Interfaces;

public interface IPayloadGlobalRepository<T> where T : class
{
    Task<T?> GetBySlugAsync(string slug, string? jwtToken = null);
    Task<IEnumerable<T>> GetAllAsync(string? jwtToken = null);
}