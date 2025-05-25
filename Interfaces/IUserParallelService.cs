using UserDistributed.Services.Metrics;
using UserDistributed.Services.Search;
using UserDistributed.Services.Statistics;
using UserDistributed.Services.UserProcessing;

namespace UserDistributed.Interfaces;

public interface IUserParallelService
{
    UserParallelProcessor UserProcessor { get; }
    CityStatisticsService CityStatsService { get; }
    UserSearchService SearchService { get; }
    UserMetricsService MetricsService { get; }
}