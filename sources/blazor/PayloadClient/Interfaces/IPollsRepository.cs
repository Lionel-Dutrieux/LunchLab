using PayloadClient.Models;
using PayloadClient.Models.Requests;

namespace PayloadClient.Interfaces;

public interface IPollsRepository : IPayloadRepository<Poll>
{
    Task<Poll?> CreatePoll(CreatePollRequest request, string? jwtToken = null);
    Task<IEnumerable<Poll>> GetActivePolls(string? jwtToken = null);
    Task<IEnumerable<Poll>> GetPollsByStatus(string status, string? jwtToken = null);
    Task<IEnumerable<Poll>> GetPollsByUser(string userId, string? jwtToken = null);
    Task<Poll?> GetPollWithDetails(string pollId, string? jwtToken = null);
    Task<Poll?> VoteOnPoll(string pollId, string optionId, string? jwtToken = null);
    Task<Poll?> ClosePoll(string pollId, string? jwtToken = null);
    Task<Poll?> AddOption(string pollId, string restaurantId, string? jwtToken = null);
    Task<Poll?> RemoveOption(string pollId, string optionId, string? jwtToken = null);
} 