using UserDistributed.DTOs;

namespace UserDistributed.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(int id);
    Task<UserDto> CreateUserAsync(CreateUserDto userDto);
    Task<bool> UpdateUserAsync(int id, UpdateUserDto userDto);
    Task<bool> DeleteUserAsync(int id);
    Task<IEnumerable<UserDto>> SearchUsersAsync(UserSearchDto searchParams);
    Task<(int totalUsers, DateTime? lastUserCreated)> GetUserStatisticsAsync();
}