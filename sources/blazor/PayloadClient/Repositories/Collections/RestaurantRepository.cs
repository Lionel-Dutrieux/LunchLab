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

    public async Task<IEnumerable<Restaurant>> GetByNameAsync(string name)
    {
        var query = new PayloadQueryBuilder()
            .Where("name", "contains", name)
            .WithDepth(1);
        
        var response = await GetWithQueryAsync<PayloadResponse<Restaurant>>(query);
        return response?.Docs ?? Enumerable.Empty<Restaurant>();
    }

    public async Task<IEnumerable<Restaurant>> GetAllWithMenuAsync()
    {
        var query = new PayloadQueryBuilder()
            .WithDepth(2)
            .Populate("menuItems")
            .Sort("name");
        
        var response = await GetWithQueryAsync<PayloadResponse<Restaurant>>(query);
        return response?.Docs ?? Enumerable.Empty<Restaurant>();
    }

    public async Task<IEnumerable<Restaurant>> SearchRestaurantsAsync(
        string? searchTerm = null, 
        int page = 1, 
        int limit = 10)
    {
        var query = new PayloadQueryBuilder()
            .WithPage(page)
            .WithLimit(limit)
            .WithDepth(1);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query.Where("name", "contains", searchTerm);
        }

        var response = await GetWithQueryAsync<PayloadResponse<Restaurant>>(query);
        return response?.Docs ?? Enumerable.Empty<Restaurant>();
    }
} 