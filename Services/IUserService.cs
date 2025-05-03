using GeekMeet.DTOs;

namespace GeekMeet.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> CreateUserAsync(CreateUserDto userDto);
        Task<bool> UpdateUserAsync(int id, UpdateUserDto userDto);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<UserDto>> SearchUsersAsync(string query);
        Task<(int totalUsers, DateTime lastUserCreated)> GetUserStatisticsAsync();
    }
} 