using UserDistributed.DTOs;
using UserDistributed.Models;

namespace UserDistributed.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<User>> SearchAsync(UserSearchDto searchParams);
        Task<(int totalUsers, DateTime? lastUserCreated)> GetStatisticsAsync();
    }
}