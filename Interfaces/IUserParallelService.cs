using GeekMeet.Services.Metrics;
using GeekMeet.Services.Search;
using GeekMeet.Services.Statistics;
using GeekMeet.Services.UserProcessing;

namespace GeekMeet.Interfaces;

public interface IUserParallelService
{
    UserParallelProcessor UserProcessor { get; }
    CityStatisticsService CityStatsService { get; }
    UserSearchService SearchService { get; }
    UserMetricsService MetricsService { get; }
} 