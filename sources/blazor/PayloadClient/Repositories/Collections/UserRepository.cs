using Microsoft.Extensions.Logging;
using PayloadClient.Interfaces;
using PayloadClient.Models;
using PayloadClient.Query;

namespace PayloadClient.Repositories.Collections;

public class UserRepository : PayloadRepository<User>, IUserRepository
{
    public UserRepository(
        IHttpClientFactory httpClientFactory,
        ILogger<PayloadRepository<User>> logger) 
        : base(httpClientFactory, "users", logger)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, string? jwtToken = null)
    {
        var query = new PayloadQueryBuilder()
            .Where("email", "equals", email)
            .WithDepth(1);
        
        var response = await GetWithQueryAsync<PayloadResponse<User>>(query, jwtToken);
        return response?.Docs?.FirstOrDefault();
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(string role, string? jwtToken = null)
    {
        var query = new PayloadQueryBuilder()
            .Where("role", "equals", role)
            .WithDepth(1)
            .Sort("lastName");
        
        var response = await GetWithQueryAsync<PayloadResponse<User>>(query, jwtToken);
        return response?.Docs ?? Enumerable.Empty<User>();
    }

    public async Task<IEnumerable<User>> SearchUsersAsync(
        string? searchTerm = null, 
        int page = 1, 
        int limit = 10, string? jwtToken = null)
    {
        var query = new PayloadQueryBuilder()
            .WithPage(page)
            .WithLimit(limit)
            .WithDepth(1)
            .Sort("lastName");

        if (!string.IsNullOrEmpty(searchTerm))
        {
            // Search in both first name and last name
            query.Or(
                q => q.Where("firstName", "contains", searchTerm),
                q => q.Where("lastName", "contains", searchTerm),
                q => q.Where("email", "contains", searchTerm)
            );
        }

        var response = await GetWithQueryAsync<PayloadResponse<User>>(query, jwtToken);
        return response?.Docs ?? Enumerable.Empty<User>();
    }
} 