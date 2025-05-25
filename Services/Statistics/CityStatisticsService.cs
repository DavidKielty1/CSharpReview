using System.Collections.Concurrent;
using UserDistributed.Interfaces;
using UserDistributed.Models;
using UserDistributed.Services.Cache;
using Microsoft.Extensions.Logging;

namespace UserDistributed.Services.Statistics;

public class CityStatisticsService(
    IUserRepository userRepository,
    IRedisService redisService,
    ILogger<CityStatisticsService> logger)
{
    public async Task<Dictionary<string, int>> GetStatisticsAsync()
    {
        var processingKey = RedisCacheKeys.StatsProcessing;
        var resultsKey = RedisCacheKeys.StatsResults;

        // Try to get cached results first
        var cachedResults = await redisService.GetAsync<Dictionary<string, int>>(resultsKey);
        if (cachedResults != null)
        {
            logger.LogInformation("Returning cached statistics");
            return cachedResults;
        }

        // Use distributed lock to prevent multiple instances from processing simultaneously
        using (await redisService.AcquireLockAsync(processingKey, TimeSpan.FromMinutes(5)))
        {
            var users = await userRepository.GetAllAsync();
            var cityStats = new ConcurrentDictionary<string, int>();

            // Process cities in parallel with a degree of parallelism limit
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 3 };

            await Parallel.ForEachAsync(users, parallelOptions, (user, token) =>
            {
                var city = user.City ?? "Unknown";
                cityStats.AddOrUpdate(city, 1, (_, count) => count + 1);
                return ValueTask.CompletedTask;
            });

            var results = cityStats.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Cache the results
            await redisService.SetAsync(resultsKey, results, TimeSpan.FromHours(1));
            logger.LogInformation("Updated and cached statistics");

            return results;
        }
    }
}