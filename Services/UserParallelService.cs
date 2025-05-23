using System.Collections.Concurrent;
using GeekMeet.DTOs;
using GeekMeet.Interfaces;
using GeekMeet.Models;

namespace GeekMeet.Services;

public class UserParallelService(
    IUserRepository userRepository,
    ILogger<UserParallelService> logger) : IUserParallelService
{
    private readonly SemaphoreSlim _semaphore = new(3); // Limit concurrent operations

    public async Task<IEnumerable<UserDto>> ProcessUsersInParallelAsync(IEnumerable<UserDto> users)
    {
        try
        {
            // Using Parallel LINQ (PLINQ) for parallel processing
            var tasks = users.AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .Select(async user =>
                {
                    await _semaphore.WaitAsync();
                    try
                    {
                        // Simulate some processing time
                        await Task.Delay(100);
                        return user;
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                });

            var processedUsers = await Task.WhenAll(tasks);
            return processedUsers;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing users in parallel");
            throw;
        }
    }

    public async Task<Dictionary<string, int>> GetUserStatisticsByCityAsync()
    {
        try
        {
            var users = await userRepository.GetAllAsync();
            
            // Using Task.WhenAll for concurrent processing
            var cityGroups = users
                .GroupBy(u => u.City ?? "Unknown")
                .Select(async group =>
                {
                    await _semaphore.WaitAsync();
                    try
                    {
                        // Simulate some processing time
                        await Task.Delay(50);
                        return new { City = group.Key, Count = group.Count() };
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                });

            var results = await Task.WhenAll(cityGroups);
            return results.ToDictionary(r => r.City, r => r.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user statistics by city");
            throw;
        }
    }

    public async Task<IEnumerable<UserDto>> GetUsersWithParallelSearchAsync(UserSearchDto searchParams)
    {
        try
        {
            var users = await userRepository.GetAllAsync();
            
            // Using Parallel.ForEach for concurrent filtering
            var filteredUsers = new ConcurrentBag<UserDto>();
            
            await Parallel.ForEachAsync(users, async (user, token) =>
            {
                if (await MatchesSearchCriteriaAsync(user, searchParams))
                {
                    filteredUsers.Add(new UserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        CreatedAt = user.CreatedAt
                    });
                }
            });

            return filteredUsers;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error performing parallel user search");
            throw;
        }
    }

    public async Task<Dictionary<string, double>> CalculateUserMetricsAsync()
    {
        try
        {
            var users = await userRepository.GetAllAsync();
            var metrics = new ConcurrentDictionary<string, double>();

            // Using Parallel.ForEach with concurrent dictionary
            await Parallel.ForEachAsync(users, async (user, token) =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    // Simulate some complex calculations
                    await Task.Delay(50);
                    
                    // Example metrics calculation
                    var daysSinceCreation = (DateTime.UtcNow - user.CreatedAt).TotalDays;
                    metrics.TryAdd($"user_{user.Id}_days_active", daysSinceCreation);
                }
                finally
                {
                    _semaphore.Release();
                }
            });

            return metrics.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating user metrics");
            throw;
        }
    }

    private async Task<bool> MatchesSearchCriteriaAsync(User user, UserSearchDto searchParams)
    {
        await Task.Delay(10); // Simulate some processing time
        
        return (string.IsNullOrEmpty(searchParams.Name) || user.Name.Contains(searchParams.Name)) &&
               (string.IsNullOrEmpty(searchParams.Email) || user.Email.Contains(searchParams.Email)) &&
               (string.IsNullOrEmpty(searchParams.City) || (user.City != null && user.City.Contains(searchParams.City))) &&
               (!searchParams.CreatedAfter.HasValue || user.CreatedAt >= searchParams.CreatedAfter) &&
               (!searchParams.CreatedBefore.HasValue || user.CreatedAt <= searchParams.CreatedBefore);
    }
} 