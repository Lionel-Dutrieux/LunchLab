using PayloadClient.Models;

namespace PayloadClient.Interfaces;

public interface IPollsRepository : IPayloadRepository<Poll>
{
    Task<IEnumerable<Poll>> GetActivePolls(string? jwtToken = null);
    Task<IEnumerable<Poll>> GetPollsByStatus(string status, string? jwtToken = null);
    Task<IEnumerable<Poll>> GetPollsByUser(string userId, string? jwtToken = null);
    Task<Poll?> GetPollWithDetails(string pollId, string? jwtToken = null);
} 