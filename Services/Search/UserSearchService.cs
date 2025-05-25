using UserDistributed.DTOs;
using UserDistributed.Interfaces;
using UserDistributed.Services.Cache;

namespace UserDistributed.Services.Search;

public class UserSearchService(
    IUserRepository userRepository,
    IRedisService redisService,
    SearchCriteriaMatcher searchCriteriaMatcher,
    ILogger<UserSearchService> logger)
{
    public async Task<IEnumerable<UserDto>> SearchUsersAsync(UserSearchDto searchParams)
    {
        var cacheKey = RedisCacheKeys.UserSearch(searchParams);

        // Try to get from Redis cache first
        var cachedResults = await redisService.GetAsync<IEnumerable<UserDto>>(cacheKey);
        if (cachedResults != null)
        {
            logger.LogInformation("Cache hit for key: {Key}", cacheKey);
            return cachedResults;
        }

        var users = await userRepository.GetAllAsync();

        // Using Parallel.ForEach for concurrent filtering
        var filteredUsers = new ConcurrentBag<UserDto>();

        await Parallel.ForEachAsync(users, async (user, token) =>
        {
            if (await searchCriteriaMatcher.MatchesAsync(user, searchParams))
            {
                filteredUsers.Add(new UserDto(
                    user.Id,
                    user.Name,
                    user.Email,
                    user.CreatedAt
                ));
            }
        });

        var results = filteredUsers.ToList();

        // Cache the results
        await redisService.SetAsync(cacheKey, results, TimeSpan.FromMinutes(15));
        logger.LogInformation("Cached search results for key: {Key}", cacheKey);

        return results;
    }
}