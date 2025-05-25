using UserDistributed.DTOs;
using UserDistributed.Services.Cache;

namespace UserDistributed.Services.UserProcessing;

public class UserParallelProcessor(IRedisService redisService, RedisWaitHelper redisWaitHelper)
{
    private readonly SemaphoreSlim _semaphore = new(3);

    public async Task<IEnumerable<UserDto>> ProcessUsersAsync(IEnumerable<UserDto> users)
    {
        var processingKey = RedisCacheKeys.Processing;
        var resultsKey = RedisCacheKeys.ProcessedResults;

        // Check if processing is already in progress
        var isProcessing = await redisService.ExistsAsync(processingKey);
        if (isProcessing)
        {
            var results = await redisWaitHelper.WaitForResultAsync<IEnumerable<UserDto>>(resultsKey);
            if (results != null)
            {
                return results;
            }
        }

        // Mark processing as started
        await redisService.SetAsync(processingKey, true, TimeSpan.FromMinutes(5));

        // Using Parallel LINQ (PLINQ) for parallel processing
        var tasks = users.AsParallel()
            .WithDegreeOfParallelism(Environment.ProcessorCount)
            .Select(async user =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    // Check if user is already processed
                    var userKey = RedisCacheKeys.UserProcessed(user.Id);
                    var cachedUser = await redisService.GetAsync<UserDto>(userKey);
                    if (cachedUser != null)
                    {
                        return cachedUser;
                    }

                    // Process user
                    await Task.Delay(100); // Simulate processing

                    // Store individual result
                    await redisService.SetAsync(userKey, user, TimeSpan.FromHours(1));

                    // Increment processed counter
                    var currentCount = await redisService.GetAsync<int?>(RedisCacheKeys.ProcessedCount);
                    await redisService.SetAsync(RedisCacheKeys.ProcessedCount,
                        (currentCount ?? 0) + 1,
                        TimeSpan.FromHours(1));

                    return user;
                }
                finally
                {
                    _semaphore.Release();
                }
            });

        var processedUsers = await Task.WhenAll(tasks);

        // Store final results
        await redisService.SetAsync(resultsKey, processedUsers, TimeSpan.FromHours(1));

        // Clear processing flag
        await redisService.RemoveAsync(processingKey);

        return processedUsers;
    }
}