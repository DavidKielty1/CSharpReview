using System.Text.Json;
using UserDistributed.DTOs;

namespace UserDistributed.Services.Cache;

public static class RedisCacheKeys
{
    public static string UserProcessed(int userId) => $"user_processed_{userId}";
    public static string UserMetrics(int userId) => $"user_metrics_{userId}";
    public static string UserSearch(UserSearchDto searchParams) => $"user_search_{JsonSerializer.Serialize(searchParams)}";
    public static string CityStats(string city) => $"city_stats_{city}";
    public static string Processing => "users_processing";
    public static string ProcessedResults => "users_processed_results";
    public static string StatsProcessing => "stats_processing";
    public static string StatsResults => "stats_results";
    public static string ProcessedCount => "processed_count";
    public static string UserMetricsCache => "user_metrics";
}