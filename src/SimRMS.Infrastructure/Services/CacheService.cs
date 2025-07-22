using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces;
using System.Text.Json;

namespace SimRMS.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            try
            {
                if (_cache.TryGetValue(key, out var value))
                {
                    if (value is T typedValue)
                        return Task.FromResult<T?>(typedValue);

                    if (value is string jsonString)
                    {
                        var deserializedValue = JsonSerializer.Deserialize<T>(jsonString);
                        return Task.FromResult<T?>(deserializedValue);
                    }
                }

                return Task.FromResult<T?>(default(T));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache key {Key}", key);
                return Task.FromResult<T?>(default(T));
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var options = new MemoryCacheEntryOptions();

                if (expiration.HasValue)
                    options.SetAbsoluteExpiration(expiration.Value);
                else
                    options.SetAbsoluteExpiration(TimeSpan.FromHours(1)); // Default 1 hour

                options.SetSlidingExpiration(TimeSpan.FromMinutes(20)); // Sliding expiration

                _cache.Set(key, value, options);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache key {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                _cache.Remove(key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache key {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(_cache.TryGetValue(key, out _));
        }

        public Task RemovePatternAsync(string pattern)
        {
            // Note: IMemoryCache doesn't support pattern removal
            // This is a simplified implementation
            // For production, consider using Redis with pattern support
            _logger.LogWarning("Pattern removal not fully supported in MemoryCache for pattern: {Pattern}", pattern);
            return Task.CompletedTask;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            var cachedValue = await GetAsync<T>(key);

            if (cachedValue != null)
                return cachedValue;

            var value = await factory();
            await SetAsync(key, value, expiration);

            return value;
        }
    }
}
