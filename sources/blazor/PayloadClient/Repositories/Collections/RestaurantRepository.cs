using PayloadClient.Interfaces;
using PayloadClient.Models;
using PayloadClient.Query;

namespace PayloadClient.Repositories.Collections;

public class RestaurantRepository : PayloadRepository<Restaurant>, IRestaurantRepository
{
    public RestaurantRepository(IHttpClientFactory httpClientFactory) 
        : base(httpClientFactory, "restaurants")
    {
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

    public async Task<IEnumerable<MenuItem>> GetMenuItemsAsync(string restaurantId)
    {
        var restaurant = await GetByIdAsync(restaurantId);
        return restaurant?.MenuItems.Docs ?? Enumerable.Empty<MenuItem>();
    }
} 