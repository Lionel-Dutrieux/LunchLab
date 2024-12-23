using Microsoft.Extensions.Logging;
using PayloadClient.Interfaces;
using PayloadClient.Models;
using PayloadClient.Query;

namespace PayloadClient.Repositories.Collections;

public class PollsRepository : PayloadRepository<Poll>, IPollsRepository
{
    public PollsRepository(
        IHttpClientFactory httpClientFactory,
        ILogger<PayloadRepository<Poll>> logger) 
        : base(httpClientFactory, "polls", logger)
    {
    }

    public async Task<IEnumerable<Poll>> GetActivePolls(string? jwtToken = null)
    {
        var query = new PayloadQueryBuilder()
            .Where("status", "equals", "active")
            .Where("endDate", "greater-than", DateTime.UtcNow)
            .WithDepth(2)
            .Sort("-createdAt");
        
        var response = await GetWithQueryAsync<PayloadResponse<Poll>>(query, jwtToken);
        return response?.Docs ?? Enumerable.Empty<Poll>();
    }

    public async Task<IEnumerable<Poll>> GetPollsByStatus(string status, string? jwtToken = null)
    {
        var query = new PayloadQueryBuilder()
            .Where("status", "equals", status)
            .WithDepth(2)
            .Sort("-createdAt");
        
        var response = await GetWithQueryAsync<PayloadResponse<Poll>>(query, jwtToken);
        return response?.Docs ?? Enumerable.Empty<Poll>();
    }

    public async Task<IEnumerable<Poll>> GetPollsByUser(string userId, string? jwtToken = null)
    {
        var query = new PayloadQueryBuilder()
            .Where("createdBy", "equals", userId)
            .WithDepth(2)
            .Sort("-createdAt");
        
        var response = await GetWithQueryAsync<PayloadResponse<Poll>>(query, jwtToken);
        return response?.Docs ?? Enumerable.Empty<Poll>();
    }

    public async Task<Poll?> GetPollWithDetails(string pollId, string? jwtToken = null)
    {
        var query = new PayloadQueryBuilder()
            .Where("id", "equals", pollId)
            .WithDepth(2);
        
        var response = await GetWithQueryAsync<PayloadResponse<Poll>>(query, jwtToken);
        return response?.Docs?.FirstOrDefault();
    }
    
    public override async Task<IEnumerable<Poll>> GetAllAsync(string? jwtToken = null)
    {
        var query = new PayloadQueryBuilder()
            .WithDepth(2);
        
        var response = await GetWithQueryAsync<PayloadResponse<Poll>>(query, jwtToken);
        return response?.Docs ?? Enumerable.Empty<Poll>();
    }
} 