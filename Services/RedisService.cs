using StackExchange.Redis;
using System.Text.Json;

namespace UserDistributed.Services;

public interface IRedisService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task<IDisposable> AcquireLockAsync(string key, TimeSpan timeout);
}

public class RedisService(IConnectionMultiplexer redis, ILogger<RedisService> logger) : IRedisService
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var value = await _db.StringGetAsync(key);
            if (value.IsNull)
                return default;

            return JsonSerializer.Deserialize<T>(value!);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting value from Redis for key {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, serializedValue, expiry);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting value in Redis for key {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing key {Key} from Redis", key);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            return await _db.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking existence of key {Key} in Redis", key);
            return false;
        }
    }

    public async Task<IDisposable> AcquireLockAsync(string key, TimeSpan timeout)
    {
        var lockKey = $"lock:{key}";
        var token = Guid.NewGuid().ToString();
        var expiry = timeout;

        var acquired = await _db.StringSetAsync(lockKey, token, expiry, When.NotExists);
        if (!acquired)
        {
            throw new TimeoutException($"Could not acquire lock for key: {key}");
        }

        return new AsyncLock(lockKey, token, _db);
    }

    private class AsyncLock : IDisposable
    {
        private readonly string _key;
        private readonly string _token;
        private readonly IDatabase _db;
        private bool _disposed;

        public AsyncLock(string key, string token, IDatabase db)
        {
            _key = key;
            _token = token;
            _db = db;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _db.KeyDelete(_key);
                _disposed = true;
            }
        }
    }
}