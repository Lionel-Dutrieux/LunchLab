using Microsoft.Extensions.Logging;
using PayloadClient.Interfaces;
using PayloadClient.Models;
using PayloadClient.Query;

namespace PayloadClient.Repositories.Collections;

public class RestaurantRepository : PayloadRepository<Restaurant>, IRestaurantRepository
{
    private readonly IMenuItemRepository _menuItemRepository;

    public RestaurantRepository(
        IHttpClientFactory httpClientFactory, 
        IMenuItemRepository menuItemRepository,
        ILogger<PayloadRepository<Restaurant>> logger) 
        : base(httpClientFactory, "restaurants", logger)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<IEnumerable<Restaurant>> GetByNameAsync(string name, string? jwtToken = null)
    {
        var query = new PayloadQueryBuilder()
            .Where("name", "contains", name)
            .WithDepth(1);
        
        var response = await GetWithQueryAsync<PayloadResponse<Restaurant>>(query, jwtToken);
        return response?.Docs ?? Enumerable.Empty<Restaurant>();
    }

    public async Task<IEnumerable<Restaurant>> GetAllWithMenuAsync(string? jwtToken = null)
    {
        var query = new PayloadQueryBuilder()
            .WithDepth(2)
            .Populate("menuItems")
            .Sort("name");
        
        var response = await GetWithQueryAsync<PayloadResponse<Restaurant>>(query, jwtToken);
        return response?.Docs ?? Enumerable.Empty<Restaurant>();
    }
} 