using System.Collections.Concurrent;
using UserDistributed.Interfaces;
using UserDistributed.Models;
using UserDistributed.Services.Cache;
using UserDistributed.Services.Metrics.Calculations;
using UserDistributed.Services.Metrics.Models;

namespace UserDistributed.Services.Metrics;

public class UserMetricsService(
    IUserRepository userRepository,
    IRedisService redisService,
    ActivityScoreCalculator activityScoreCalculator,
    EngagementRateCalculator engagementRateCalculator,
    PerformanceIndexCalculator performanceIndexCalculator,
    ILogger<UserMetricsService> logger)
{
    private readonly SemaphoreSlim _semaphore = new(3);

    public async Task<Dictionary<string, UserMetrics>> CalculateMetricsAsync()
    {
        var cacheKey = RedisCacheKeys.UserMetricsCache;

        // Try to get from Redis cache first
        var cachedMetrics = await redisService.GetAsync<Dictionary<string, UserMetrics>>(cacheKey);
        if (cachedMetrics != null)
        {
            logger.LogInformation("Cache hit for key: {Key}", cacheKey);
            return cachedMetrics;
        }

        var users = await userRepository.GetAllAsync();
        var metrics = new ConcurrentDictionary<string, UserMetrics>();

        // Using Parallel.ForEach with concurrent dictionary
        await Parallel.ForEachAsync(users, async (user, token) =>
        {
            await _semaphore.WaitAsync();
            try
            {
                // Check if metrics are already calculated for this user
                var userMetricsKey = RedisCacheKeys.UserMetrics(user.Id);
                var cachedUserMetrics = await redisService.GetAsync<UserMetrics>(userMetricsKey);
                if (cachedUserMetrics != null)
                {
                    metrics.TryAdd($"user_{user.Id}", cachedUserMetrics);
                    return;
                }

                // Simulate some complex calculations
                await Task.Delay(50);

                // Calculate metrics using the model
                var userMetrics = new UserMetrics
                {
                    DaysActive = (DateTime.UtcNow - user.CreatedAt).TotalDays,
                    ActivityScore = activityScoreCalculator.Calculate(user),
                    EngagementRate = engagementRateCalculator.Calculate(user),
                    PerformanceIndex = performanceIndexCalculator.Calculate(user),
                    CalculatedAt = DateTime.UtcNow
                };

                // Cache individual user metrics
                await redisService.SetAsync(userMetricsKey, userMetrics, TimeSpan.FromHours(1));

                // Add to the main metrics dictionary
                metrics.TryAdd($"user_{user.Id}", userMetrics);
            }
            finally
            {
                _semaphore.Release();
            }
        });

        var result = metrics.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        // Cache the complete metrics
        await redisService.SetAsync(cacheKey, result, TimeSpan.FromHours(1));
        logger.LogInformation("Cached metrics for key: {Key}", cacheKey);

        return result;
    }
}