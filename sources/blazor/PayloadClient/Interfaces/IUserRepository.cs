using PayloadClient.Models;

namespace PayloadClient.Interfaces;

public interface IUserRepository : IPayloadRepository<User>
{
    Task<User?> GetByEmailAsync(string email, string? jwtToken = null);
    Task<IEnumerable<User>> GetByRoleAsync(string role, string? jwtToken = null);
    Task<IEnumerable<User>> GetAllAsync(string? jwtToken = null);
    Task<IEnumerable<User>> SearchUsersAsync(string? searchTerm = null, int page = 1, int limit = 10, string? jwtToken = null);
} 