using Microsoft.Extensions.Logging;
using PayloadClient.Helpers;
using PayloadClient.Interfaces;
using PayloadClient.Models;
using PayloadClient.Models.Requests;
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

    public async Task<Poll?> CreatePoll(CreatePollRequest request, string? jwtToken = null)
    {
        var pollData = new
        {
            title = request.Title,
            status = "active",
            endDate = request.EndDate,
            options = request.RestaurantIds.Select(restaurantId => new
            {
                restaurant = restaurantId
            }).ToArray()
        };

        return await CreateAsync(pollData, jwtToken);
    }

    public async Task<Poll?> AddOption(string pollId, string restaurantId, string? jwtToken = null)
    {
        // Get current poll to preserve existing options
        var currentPoll = await GetPollWithDetails(pollId, jwtToken);
        if (currentPoll == null) return null;

        var update = new
        {
            options = currentPoll.Options
                .Select(o => new { restaurant = o.Restaurant.Id })
                .Append(new { restaurant = restaurantId })
                .ToArray()
        };

        return await UpdateAsync(pollId, update, jwtToken);
    }

    public async Task<Poll?> RemoveOption(string pollId, string optionId, string? jwtToken = null)
    {
        var update = new
        {
            options = new[]
            {
                new { id = optionId, _delete = true }
            }
        };
        
        return await UpdateAsync(pollId, update, jwtToken);
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

    public async Task<Poll?> VoteOnPoll(string pollId, string optionId, string? jwtToken = null)
    {
        var currentPoll = await GetPollWithDetails(pollId, jwtToken);
        if (currentPoll == null) return null;

        var userId = JwtTokenHelper.GetUserIdFromToken(jwtToken);
        if (string.IsNullOrEmpty(userId)) return null;

        // Find and update the specific option
        var option = currentPoll.Options.FirstOrDefault(o => o.Id == optionId);
        if (option == null) return null;

        option.Votes.Add(new PollVote
        {
            User = new UserRef { Id = userId },
            VotedAt = DateTime.UtcNow
        });

        return await UpdateAsync(pollId, currentPoll, jwtToken);
    }

    public async Task<Poll?> ClosePoll(string pollId, string? jwtToken = null)
    {
        return await UpdateAsync(pollId, new { status = "closed" }, jwtToken);
    }
    
    public override async Task<IEnumerable<Poll>> GetAllAsync(string? jwtToken = null)
    {
        var query = new PayloadQueryBuilder()
            .WithDepth(2);
        
        var response = await GetWithQueryAsync<PayloadResponse<Poll>>(query, jwtToken);
        return response?.Docs ?? Enumerable.Empty<Poll>();
    }
} 