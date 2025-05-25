using UserDistributed.Interfaces;
using UserDistributed.Services.Metrics;
using UserDistributed.Services.Search;
using UserDistributed.Services.Statistics;
using UserDistributed.Services.UserProcessing;

namespace UserDistributed.Services;

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