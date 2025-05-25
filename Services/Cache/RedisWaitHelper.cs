namespace UserDistributed.Services.Cache;

public class RedisWaitHelper(IRedisService redisService)
{
    private readonly TimeSpan _maxWaitTime = TimeSpan.FromMinutes(5);

    public async Task<T?> WaitForResultAsync<T>(string resultsKey)
    {
        var startTime = DateTime.UtcNow;

        while (DateTime.UtcNow - startTime < _maxWaitTime)
        {
            var results = await redisService.GetAsync<T>(resultsKey);
            if (results != null)
            {
                return results;
            }
            await Task.Delay(1000);
        }

        return default;
    }
}