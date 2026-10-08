using Examprep.Domain.Model;

namespace Examprep.Application.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User> AddAsync(User user);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task UpdateAsync(User user);
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task DeleteAsync(User user);
    }
}