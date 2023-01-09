// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public interface IMultilevelCacheClient : ICacheClient
{
    /// <summary>
    /// Get cache
    /// When the memory cache does not exist, get the result of the distributed cache and store the result in the memory cache (the validity period of the memory cache is the expiration time passed in)
    /// </summary>
    /// <param name="key"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T? Get<T>(string key, Action<MultilevelCacheOptions>? action = null);

    /// <summary>
    /// Get cache
    /// When the memory cache does not exist, get the result of the distributed cache and store the result in the memory cache (the validity period of the memory cache is the expiration time passed in)
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="valueChanged"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T? Get<T>(string key, Action<T?> valueChanged, Action<MultilevelCacheOptions>? action = null);

    /// <summary>
    /// Get cache
    /// When the memory cache does not exist, get the result of the distributed cache and store the result in the memory cache (the validity period of the memory cache is the expiration time passed in)
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T?> GetAsync<T>(string key, Action<MultilevelCacheOptions>? action = null);

    /// <summary>
    /// Get cache
    /// When the memory cache does not exist, get the result of the distributed cache and store the result in the memory cache (the validity period of the memory cache is the expiration time passed in)
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="valueChanged"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T?> GetAsync<T>(string key, Action<T?> valueChanged, Action<MultilevelCacheOptions>? action = null);

    /// <summary>
    /// Get cache collection
    /// When the memory cache does not exist, get the result of the distributed cache and store the result in the memory cache (the validity period of the memory cache is the expiration time passed in)
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IEnumerable<T?> GetList<T>(IEnumerable<string> keys, Action<MultilevelCacheOptions>? action = null);

    /// <summary>
    /// Get cache collection
    /// When the memory cache does not exist, get the result of the distributed cache and store the result in the memory cache (the validity period of the memory cache is the expiration time passed in)
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IEnumerable<T?>> GetListAsync<T>(IEnumerable<string> keys, Action<MultilevelCacheOptions>? action = null);

    /// <summary>
    /// Get cache, set cache if cache does not exist
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="distributedCacheEntryFunc">Distributed cache information returned when the memory cache does not exist</param>
    /// <param name="memoryCacheEntryOptions">Memory cache lifetime configuration，which is consistent with the default configuration when it is empty</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T? GetOrSet<T>(string key,
        Func<CacheEntry<T>> distributedCacheEntryFunc,
        CacheEntryOptions? memoryCacheEntryOptions = null,
        Action<CacheOptions>? action = null);

    /// <summary>
    /// Get cache, set cache if cache does not exist
    /// </summary>
    /// <param name="key">Cache key, the actual cache key will decide whether to format the cache key according to the global configuration and Action</param>
    /// <param name="combinedCacheEntry">Cache key information, used to configure the execution of Handler when the cache does not exist, and the memory cache life cycle</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T? GetOrSet<T>(string key, CombinedCacheEntry<T> combinedCacheEntry, Action<CacheOptions>? action = null);

    /// <summary>
    /// Get cache, set cache if cache does not exist
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="distributedCacheEntryFunc">Distributed cache information returned when the memory cache does not exist</param>
    /// <param name="memoryCacheEntryOptions">Memory cache lifetime configuration，which is consistent with the default configuration when it is empty</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T?> GetOrSetAsync<T>(
        string key,
        Func<CacheEntry<T>> distributedCacheEntryFunc,
        CacheEntryOptions? memoryCacheEntryOptions = null,
        Action<CacheOptions>? action = null);

    /// <summary>
    /// Get cache, set cache if cache does not exist
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="distributedCacheEntryFunc">Distributed cache information returned when the memory cache does not exist</param>
    /// <param name="memoryCacheEntryOptions">Memory cache lifetime configuration，which is consistent with the default configuration when it is empty</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T?> GetOrSetAsync<T>(
        string key,
        Func<Task<CacheEntry<T>>> distributedCacheEntryFunc,
        CacheEntryOptions? memoryCacheEntryOptions = null,
        Action<CacheOptions>? action = null);

    Task<T?> GetOrSetAsync<T>(string key, CombinedCacheEntry<T> combinedCacheEntry, Action<CacheOptions>? action = null);

    /// <summary>
    /// Flush cache time to live
    /// </summary>
    /// <param name="keys">Set of cache keys</param>
    void Refresh<T>(params string[] keys);

    /// <summary>
    /// Flush cache time to live
    /// </summary>
    /// <param name="keys">Set of cache keys</param>
    Task RefreshAsync<T>(params string[] keys);

    void Remove<T>(params string[] keys);

    Task RemoveAsync<T>(params string[] keys);

    void Set<T>(string key,
        T value,
        CacheEntryOptions? distributedOptions,
        CacheEntryOptions? memoryOptions,
        Action<CacheOptions>? action = null);

    void Set<T>(string key, T value, CombinedCacheEntryOptions? options, Action<CacheOptions>? action = null);

    Task SetAsync<T>(
        string key,
        T value,
        CacheEntryOptions? distributedOptions,
        CacheEntryOptions? memoryOptions,
        Action<CacheOptions>? action = null);

    Task SetAsync<T>(string key, T value, CombinedCacheEntryOptions? options, Action<CacheOptions>? action = null);

    void SetList<T>(
        Dictionary<string, T?> keyValues,
        CacheEntryOptions? distributedOptions,
        CacheEntryOptions? memoryOptions,
        Action<CacheOptions>? action = null);

    void SetList<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions? options, Action<CacheOptions>? action = null);

    Task SetListAsync<T>(
        Dictionary<string, T?> keyValues,
        CacheEntryOptions? distributedOptions,
        CacheEntryOptions? memoryOptions,
        Action<CacheOptions>? action = null);

    Task SetListAsync<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions? options, Action<CacheOptions>? action = null);
}
