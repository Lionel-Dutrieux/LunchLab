using PayloadClient.Models;

namespace PayloadClient.Interfaces;

public interface IMenuItemRepository : IPayloadRepository<MenuItem>
{
    Task<IEnumerable<MenuItem>> GetByRestaurantIdAsync(string restaurantId);
    Task<IEnumerable<MenuItem>> GetByNameAsync(string name);
} 