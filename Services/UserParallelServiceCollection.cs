using GeekMeet.Interfaces;
using GeekMeet.Services.Metrics;
using GeekMeet.Services.Search;
using GeekMeet.Services.Statistics;
using GeekMeet.Services.UserProcessing;

namespace GeekMeet.Services;

public class UserParallelServiceCollection : IUserParallelService
{
    public UserParallelProcessor UserProcessor { get; }
    public CityStatisticsService CityStatsService { get; }
    public UserSearchService SearchService { get; }
    public UserMetricsService MetricsService { get; }

    public UserParallelServiceCollection(
        UserParallelProcessor userProcessor,
        CityStatisticsService cityStatsService,
        UserSearchService searchService,
        UserMetricsService metricsService)
    {
        UserProcessor = userProcessor;
        CityStatsService = cityStatsService;
        SearchService = searchService;
        MetricsService = metricsService;
    }
} 