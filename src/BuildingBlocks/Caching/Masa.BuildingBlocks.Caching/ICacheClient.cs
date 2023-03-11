// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public interface ICacheClient : IDisposable
{
    IEnumerable<T?> GetList<T>(params string[] keys);

    Task<IEnumerable<T?>> GetListAsync<T>(params string[] keys);

    /// <summary>
    /// Set cache
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="value">Cache value</param>
    /// <param name="absoluteExpiration">Absolute Expiration，Permanently valid when null</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    void Set<T>(string key, T value, DateTimeOffset? absoluteExpiration, Action<CacheOptions>? action = null);

    /// <summary>
    /// Set cache
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="value">Cache value</param>
    /// <param name="absoluteExpirationRelativeToNow">Absolute Expiration Relative To Now，Permanently valid when null</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    void Set<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null);

    /// <summary>
    /// Set cache
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="value">Cache value</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    void Set<T>(string key, T value, CacheEntryOptions? options = null, Action<CacheOptions>? action = null);

    /// <summary>
    /// Set cache
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="value">Cache value</param>
    /// <param name="absoluteExpiration">Absolute Expiration，Permanently valid when null</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    Task SetAsync<T>(string key, T value, DateTimeOffset? absoluteExpiration, Action<CacheOptions>? action = null);

    /// <summary>
    /// Set cache
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="value">Cache value</param>
    /// <param name="absoluteExpirationRelativeToNow">Absolute Expiration Relative To Now，Permanently valid when null</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null);

    /// <summary>
    /// Set cache
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="value">Cache value</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    Task SetAsync<T>(string key, T value, CacheEntryOptions? options = null, Action<CacheOptions>? action = null);

    /// <summary>
    /// Batch setting cache
    /// </summary>
    /// <param name="keyValues">A collection of key-value pairs</param>
    /// <param name="absoluteExpiration">Absolute Expiration，Permanently valid when null</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    void SetList<T>(Dictionary<string, T?> keyValues, DateTimeOffset? absoluteExpiration, Action<CacheOptions>? action = null);

    /// <summary>
    /// Batch setting cache
    /// </summary>
    /// <param name="keyValues">A collection of key-value pairs</param>
    /// <param name="absoluteExpirationRelativeToNow">Absolute Expiration Relative To Now，Permanently valid when null</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    void SetList<T>(Dictionary<string, T?> keyValues, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null);

    /// <summary>
    /// Batch setting cache
    /// </summary>
    /// <param name="keyValues">A collection of key-value pairs</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    void SetList<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? options = null, Action<CacheOptions>? action = null);

    /// <summary>
    /// Batch setting cache
    /// </summary>
    /// <param name="keyValues">A collection of key-value pairs</param>
    /// <param name="absoluteExpiration">Absolute Expiration，Permanently valid when null</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    Task SetListAsync<T>(Dictionary<string, T?> keyValues, DateTimeOffset? absoluteExpiration, Action<CacheOptions>? action = null);

    /// <summary>
    /// Batch setting cache
    /// </summary>
    /// <param name="keyValues">A collection of key-value pairs</param>
    /// <param name="absoluteExpirationRelativeToNow">Absolute Expiration Relative To Now，Permanently valid when null</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    Task SetListAsync<T>(Dictionary<string, T?> keyValues, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null);

    /// <summary>
    /// Batch setting cache
    /// </summary>
    /// <param name="keyValues">A collection of key-value pairs</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    Task SetListAsync<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? options = null, Action<CacheOptions>? action = null);

    /// <summary>
    /// delete cache key
    /// </summary>
    /// <param name="key">cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    void Remove<T>(string key, Action<CacheOptions>? action = null);

    /// <summary>
    /// delete cache key set
    /// </summary>
    /// <param name="keys">A collection of cache keys, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    void Remove<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null);

    /// <summary>
    /// delete cache key
    /// </summary>
    /// <param name="key">cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task RemoveAsync<T>(string key, Action<CacheOptions>? action = null);

    /// <summary>
    /// delete cache key set
    /// </summary>
    /// <param name="keys">A collection of cache keys, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task RemoveAsync<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null);

    /// <summary>
    /// Flush cache time to live
    /// </summary>
    /// <param name="keys">A collection of cache keys, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    void Refresh<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null);

    /// <summary>
    /// Flush cache time to live
    /// </summary>
    /// <param name="keys">A collection of cache keys, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    Task RefreshAsync<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null);
}
