using PayloadClient.Interfaces;
using PayloadClient.Models;
using PayloadClient.Query;
using Microsoft.Extensions.Logging;

namespace PayloadClient.Repositories.Collections;

public class MenuItemRepository : PayloadRepository<MenuItem>, IMenuItemRepository
{
    public MenuItemRepository(
        IHttpClientFactory httpClientFactory,
        ILogger<PayloadRepository<MenuItem>> logger) 
        : base(httpClientFactory, "menu-items", logger)
    {
    }

    public async Task<IEnumerable<MenuItem>> GetByRestaurantIdAsync(string restaurantId)
    {
        var query = new PayloadQueryBuilder()
            .Where("restaurant", "equals", restaurantId)
            .WithDepth(1);
        
        var response = await GetWithQueryAsync<PayloadResponse<MenuItem>>(query);
        return response?.Docs ?? Enumerable.Empty<MenuItem>();
    }

    public async Task<IEnumerable<MenuItem>> GetByNameAsync(string name)
    {
        var query = new PayloadQueryBuilder()
            .Where("name", "contains", name)
            .WithDepth(1);
        
        var response = await GetWithQueryAsync<PayloadResponse<MenuItem>>(query);
        return response?.Docs ?? Enumerable.Empty<MenuItem>();
    }
} 