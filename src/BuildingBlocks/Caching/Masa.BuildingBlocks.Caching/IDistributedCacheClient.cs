// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public interface IDistributedCacheClient : ICacheClient
{
    T? Get<T>(string key, Action<CacheOptions>? action = null);

    Task<T?> GetAsync<T>(string key, Action<CacheOptions>? action = null);

    IEnumerable<T?> GetList<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null);

    Task<IEnumerable<T?>> GetListAsync<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null);

    T? GetOrSet<T>(string key, Func<CacheEntry<T>> setter, Action<CacheOptions>? action = null);

    Task<T?> GetOrSetAsync<T>(string key, Func<CacheEntry<T>> setter, Action<CacheOptions>? action = null);

    /// <summary>
    /// Flush cache time to live
    /// </summary>
    /// <param name="keys"></param>
    void Refresh(params string[] keys);

    /// <summary>
    /// Flush cache time to live
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    Task RefreshAsync(params string[] keys);

    /// <summary>
    /// Flush cache time to live
    /// </summary>
    /// <param name="keys">A collection of cache keys</param>
    void Remove(params string[] keys);

    /// <summary>
    /// Remove cache key
    /// </summary>
    /// <param name="keys">A collection of cache keys</param>
    /// <returns></returns>
    Task RemoveAsync(params string[] keys);

    /// <summary>
    /// Get whether the cache key exists
    /// </summary>
    /// <param name="key">Complete cache key, cache key is no longer formatted</param>
    /// <returns></returns>
    bool Exists(string key);

    /// <summary>
    /// Get whether the cache key exists
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    bool Exists<T>(string key, Action<CacheOptions>? action = null);

    /// <summary>
    /// Get whether the cache key exists
    /// </summary>
    /// <param name="key">Complete cache key, cache key is no longer formatted</param>
    /// <returns></returns>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// Get whether the cache key exists
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<bool> ExistsAsync<T>(string key, Action<CacheOptions>? action = null);

    /// <summary>
    /// Only get the key, do not trigger the update expiration time policy
    /// </summary>
    /// <param name="keyPattern">Complete keyPattern, no longer formatted, such as: app_*</param>
    /// <returns></returns>
    IEnumerable<string> GetKeys(string keyPattern);

    /// <summary>
    /// The set of cached keys that match the rules according to the key fuzzy matching
    /// Obtain the set of keys that meet the rules according to the obtained type and KeyPattern
    /// </summary>
    /// <param name="keyPattern">keyPattern, used to change the global cache configuration information, eg: 1*</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IEnumerable<string> GetKeys<T>(string keyPattern, Action<CacheOptions>? action = null);

    /// <summary>
    /// Only get the key, do not trigger the update expiration time policy
    /// </summary>
    /// <param name="keyPattern">Complete keyPattern, no longer formatted, such as: app_*</param>
    /// <returns></returns>
    Task<IEnumerable<string>> GetKeysAsync(string keyPattern);

    /// <summary>
    /// The set of cached keys that match the rules according to the key fuzzy matching
    /// Obtain the set of keys that meet the rules according to the obtained type and KeyPattern
    /// </summary>
    /// <param name="keyPattern">keyPattern, used to change the global cache configuration information, eg: 1*</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IEnumerable<string>> GetKeysAsync<T>(string keyPattern, Action<CacheOptions>? action = null);

    IEnumerable<KeyValuePair<string, T?>> GetByKeyPattern<T>(string keyPattern, Action<CacheOptions>? action = null);

    Task<IEnumerable<KeyValuePair<string, T?>>> GetByKeyPatternAsync<T>(string keyPattern, Action<CacheOptions>? action = null);

    void Publish(string channel, Action<PublishOptions> options);

    Task PublishAsync(string channel, Action<PublishOptions> options);

    void Subscribe<T>(string channel, Action<SubscribeOptions<T>> options);

    Task SubscribeAsync<T>(string channel, Action<SubscribeOptions<T>> options);

    void UnSubscribe<T>(string channel);

    Task UnSubscribeAsync<T>(string channel);

    /// <summary>
    /// Increment Hash
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="value">incremental increment, must be greater than 0</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty (is only initialized if the configuration does not exist)</param>
    /// <returns>Returns the field value after the increment operation</returns>
    Task<long> HashIncrementAsync(string key,
        long value = 1L,
        Action<CacheOptions>? action = null,
        CacheEntryOptions? options = null);

    /// <summary>
    /// Descending Hash
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="value">decrement increment, must be greater than 0</param>
    /// <param name="defaultMinVal">critical value</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty (is only initialized if the configuration does not exist)</param>
    /// <returns>Returns null on failure, and returns the field value after the decrement operation on success</returns>
    Task<long?> HashDecrementAsync(string key,
        long value = 1L,
        long defaultMinVal = 0L,
        Action<CacheOptions>? action = null,
        CacheEntryOptions? options = null);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">Complete cache key</param>
    /// <param name="absoluteExpirationRelativeToNow">absolute Expiration Relative To Now，Permanently valid when null</param>
    /// <returns></returns>
    bool KeyExpire(string key, TimeSpan? absoluteExpirationRelativeToNow);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="absoluteExpirationRelativeToNow">absolute Expiration Relative To Now，Permanently valid when null</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <returns></returns>
    bool KeyExpire<T>(string key, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">Complete cache key</param>
    /// <param name="absoluteExpiration">absolute Expiration，Permanently valid when null</param>
    /// <returns></returns>
    bool KeyExpire(string key, DateTimeOffset absoluteExpiration);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="absoluteExpiration">absolute Expiration，Permanently valid when null</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <returns></returns>
    bool KeyExpire<T>(string key, DateTimeOffset absoluteExpiration, Action<CacheOptions>? action = null);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">Complete cache key</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <returns></returns>
    bool KeyExpire(string key, CacheEntryOptions? options = null);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <returns></returns>
    bool KeyExpire<T>(string key, CacheEntryOptions? options = null, Action<CacheOptions>? action = null);

    /// <summary>
    /// Refresh the lifetime of the cache key set
    /// </summary>
    /// <param name="keys">A collection of cache keys</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <returns>Get the number of caches that have successfully refreshed the cache key life cycle</returns>
    long KeyExpire(IEnumerable<string> keys, CacheEntryOptions? options = null);

    /// <summary>
    /// Refresh the lifetime of the cache key set
    /// </summary>
    /// <param name="keys">A collection of cache keys, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Get the number of caches that have successfully refreshed the cache key life cycle</returns>
    long KeyExpire<T>(IEnumerable<string> keys, CacheEntryOptions? options = null, Action<CacheOptions>? action = null);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">Complete cache key</param>
    /// <param name="absoluteExpirationRelativeToNow">absolute Expiration Relative To Now，Permanently valid when null</param>
    /// <returns></returns>
    Task<bool> KeyExpireAsync(string key, TimeSpan? absoluteExpirationRelativeToNow);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="absoluteExpirationRelativeToNow">absolute Expiration Relative To Now，Permanently valid when null</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <returns></returns>
    Task<bool> KeyExpireAsync<T>(string key, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">Complete cache key</param>
    /// <param name="absoluteExpiration">absolute Expiration，Permanently valid when null</param>
    /// <returns></returns>
    Task<bool> KeyExpireAsync(string key, DateTimeOffset absoluteExpiration);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="absoluteExpiration">absolute Expiration，Permanently valid when null</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <returns></returns>
    Task<bool> KeyExpireAsync<T>(string key, DateTimeOffset absoluteExpiration, Action<CacheOptions>? action = null);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">Complete cache key</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <returns></returns>
    Task<bool> KeyExpireAsync(string key, CacheEntryOptions? options = null);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <returns></returns>
    Task<bool> KeyExpireAsync<T>(string key, CacheEntryOptions? options = null, Action<CacheOptions>? action = null);

    /// <summary>
    /// Batch setting cache lifetime
    /// </summary>
    /// <param name="keys">A collection of cache keys</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <returns></returns>
    Task<long> KeyExpireAsync(IEnumerable<string> keys, CacheEntryOptions? options = null);

    /// <summary>
    /// Batch setting cache lifetime
    /// </summary>
    /// <param name="keys">A collection of cache keys, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <returns></returns>
    Task<long> KeyExpireAsync<T>(IEnumerable<string> keys, CacheEntryOptions? options = null, Action<CacheOptions>? action = null);
}
