using System.Collections.Concurrent;
using GeekMeet.Interfaces;
using GeekMeet.Models;
using GeekMeet.Services.Cache;

namespace GeekMeet.Services.Statistics;

public class CityStatisticsService
{
    private readonly IUserRepository _userRepository;
    private readonly IRedisService _redisService;
    private readonly RedisWaitHelper _redisWaitHelper;

    public CityStatisticsService(
        IUserRepository userRepository,
        IRedisService redisService,
        RedisWaitHelper redisWaitHelper)
    {
        _userRepository = userRepository;
        _redisService = redisService;
        _redisWaitHelper = redisWaitHelper;
    }

    public async Task<Dictionary<string, int>> GetStatisticsAsync()
    {
        var processingKey = RedisCacheKeys.StatsProcessing;
        var resultsKey = RedisCacheKeys.StatsResults;
        
        // Check if processing is already in progress
        var isProcessing = await _redisService.ExistsAsync(processingKey);
        if (isProcessing)
        {
            var statsResults = await _redisWaitHelper.WaitForResultAsync<Dictionary<string, int>>(resultsKey);
            if (statsResults != null)
            {
                return statsResults;
            }
        }

        // Mark processing as started
        await _redisService.SetAsync(processingKey, true, TimeSpan.FromMinutes(5));
        
        var users = await _userRepository.GetAllAsync();
        var cityStats = new ConcurrentDictionary<string, int>();
        
        // Process cities in parallel
        await Parallel.ForEachAsync(users, async (user, token) =>
        {
            var city = user.City ?? "Unknown";
            
            // Use Redis to coordinate updates
            var cityKey = RedisCacheKeys.CityStats(city);
            var currentCityCount = await _redisService.GetAsync<int?>(cityKey);
            await _redisService.SetAsync(cityKey, (currentCityCount ?? 0) + 1, TimeSpan.FromHours(1));
            
            cityStats.AddOrUpdate(city, 1, (_, count) => count + 1);
        });

        var results = cityStats.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        
        // Store final results
        await _redisService.SetAsync(resultsKey, results, TimeSpan.FromHours(1));
        
        // Clear processing flag
        await _redisService.RemoveAsync(processingKey);
        
        return results;
    }
} 