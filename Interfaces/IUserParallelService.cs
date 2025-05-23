using GeekMeet.DTOs;

namespace GeekMeet.Interfaces;

public interface IUserParallelService
{
    Task<IEnumerable<UserDto>> ProcessUsersInParallelAsync(IEnumerable<UserDto> users);
    Task<Dictionary<string, int>> GetUserStatisticsByCityAsync();
    Task<IEnumerable<UserDto>> GetUsersWithParallelSearchAsync(UserSearchDto searchParams);
    Task<Dictionary<string, double>> CalculateUserMetricsAsync();
} 