using GeekMeet.Models;

namespace GeekMeet.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<User>> SearchAsync(string query);
        Task<(int totalUsers, DateTime lastUserCreated)> GetStatisticsAsync();
    }
} 