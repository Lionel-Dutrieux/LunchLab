using PayloadClient.Models;

namespace PayloadClient.Interfaces;

public interface IMenuItemRepository : IPayloadRepository<MenuItem>
{
    Task<IEnumerable<MenuItem>> GetByRestaurantIdAsync(string restaurantId, string? jwtToken = null);
    Task<IEnumerable<MenuItem>> GetByNameAsync(string name, string? jwtToken = null);
} 