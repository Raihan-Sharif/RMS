using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimRMS.Application.Interfaces;
using SimRMS.Shared.Configuration;
using System.Collections.Concurrent;
using System.Text.Json;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       Cache Service
/// Author:      Md. Raihan Sharif
/// Purpose:     This service provides caching functionality using IMemoryCache for efficient data retrieval and storage.
/// Creation:    03/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>
/// 

namespace SimRMS.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheService> _logger;
        private readonly CacheConfiguration _cacheConfig;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
        private readonly ConcurrentHashSet<string> _cacheKeys = new();

        public CacheService(IMemoryCache cache, ILogger<CacheService> logger, IOptions<CacheConfiguration> cacheConfig)
        {
            _cache = cache;
            _logger = logger;
            _cacheConfig = cacheConfig.Value;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                if (_cache.TryGetValue(key, out var value))
                {
                    if (value is T typedValue)
                        return typedValue;

                    if (value is string jsonString)
                        return JsonSerializer.Deserialize<T>(jsonString);
                }

                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache GET error for key: {Key}", key);
                return default(T);
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? _cacheConfig.AbsoluteExpiration,
                    SlidingExpiration = _cacheConfig.SlidingExpiration,
                    Priority = CacheItemPriority.High
                    // REMOVED: Size property to avoid conflicts
                };

                // Add eviction callback for cleanup
                options.RegisterPostEvictionCallback((k, v, reason, state) =>
                {
                    _cacheKeys.TryRemove(k.ToString()!);
                    _locks.TryRemove(k.ToString()!, out _);
                });

                _cache.Set(key, value, options);
                _cacheKeys.Add(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache SET error for key: {Key}", key);
            }
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            var cachedValue = await GetAsync<T>(key);
            if (cachedValue != null)
                return cachedValue;

            // Prevent cache stampede using semaphore
            var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

            await semaphore.WaitAsync();
            try
            {
                // Double-check after acquiring lock
                cachedValue = await GetAsync<T>(key);
                if (cachedValue != null)
                    return cachedValue;

                var value = await factory();
                await SetAsync(key, value, expiration);
                return value;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            _cacheKeys.TryRemove(key);
            _locks.TryRemove(key, out _);
        }

        public async Task RemovePatternAsync(string pattern)
        {
            var keysToRemove = _cacheKeys.Where(k => k.Contains(pattern)).ToList();
            foreach (var key in keysToRemove)
            {
                await RemoveAsync(key);
            }
        }

        public Task<bool> ExistsAsync(string key) => Task.FromResult(_cache.TryGetValue(key, out _));
    }

    // Thread-safe HashSet for tracking cache keys
    public class ConcurrentHashSet<T> : IDisposable
    {
        private readonly HashSet<T> _hashSet = new();
        private readonly ReaderWriterLockSlim _lock = new();

        public void Add(T item)
        {
            _lock.EnterWriteLock();
            try { _hashSet.Add(item); }
            finally { _lock.ExitWriteLock(); }
        }

        public bool TryRemove(T item)
        {
            _lock.EnterWriteLock();
            try { return _hashSet.Remove(item); }
            finally { _lock.ExitWriteLock(); }
        }

        public IEnumerable<T> Where(Func<T, bool> predicate)
        {
            _lock.EnterReadLock();
            try { return _hashSet.Where(predicate).ToList(); }
            finally { _lock.ExitReadLock(); }
        }

        public void Dispose() => _lock?.Dispose();
    }
}