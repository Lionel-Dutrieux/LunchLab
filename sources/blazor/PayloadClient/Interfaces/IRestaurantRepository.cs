using PayloadClient.Models;

namespace PayloadClient.Interfaces;

public interface IRestaurantRepository : IPayloadRepository<Restaurant>
{
    // Add any restaurant-specific methods here
    Task<IEnumerable<Restaurant>> GetByNameAsync(string name, string? jwtToken = null);
} 