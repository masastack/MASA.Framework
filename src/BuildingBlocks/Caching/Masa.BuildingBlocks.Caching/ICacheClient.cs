// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public interface ICacheClient
{
    T? Get<T>(string key);

    Task<T?> GetAsync<T>(string key);

    IEnumerable<T?> GetList<T>(params string[] keys);

    IEnumerable<T?> GetList<T>(IEnumerable<string> keys);

    Task<IEnumerable<T?>> GetListAsync<T>(params string[] keys);

    Task<IEnumerable<T?>> GetListAsync<T>(IEnumerable<string> keys);

    /// <summary>
    /// set cache
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="value">cache value</param>
    /// <param name="absoluteExpiration">absolute Expiration，Permanently valid when null</param>
    /// <typeparam name="T"></typeparam>
    void Set<T>(string key, T value, DateTimeOffset? absoluteExpiration);

    /// <summary>
    /// set cache
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="value">cache value</param>
    /// <param name="absoluteExpirationRelativeToNow">absolute Expiration Relative To Now，Permanently valid when null</param>
    /// <typeparam name="T"></typeparam>
    void Set<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow);

    /// <summary>
    /// set cache
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="value">cache value</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <typeparam name="T"></typeparam>
    void Set<T>(string key, T value, CacheEntryOptions? options = null);

    /// <summary>
    /// set cache
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="value">cache value</param>
    /// <param name="absoluteExpiration">absolute Expiration，Permanently valid when null</param>
    /// <typeparam name="T"></typeparam>
    Task SetAsync<T>(string key, T value, DateTimeOffset? absoluteExpiration);

    /// <summary>
    /// set cache
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="value">cache value</param>
    /// <param name="absoluteExpirationRelativeToNow">absolute Expiration Relative To Now，Permanently valid when null</param>
    /// <typeparam name="T"></typeparam>
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow);

    /// <summary>
    /// set cache
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="value">cache value</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <typeparam name="T"></typeparam>
    Task SetAsync<T>(string key, T value, CacheEntryOptions? options = null);

    /// <summary>
    /// Batch setting cache
    /// </summary>
    /// <param name="keyValues">A collection of key-value pairs</param>
    /// <param name="absoluteExpiration">absolute Expiration，Permanently valid when null</param>
    /// <typeparam name="T"></typeparam>
    void SetList<T>(Dictionary<string, T?> keyValues, DateTimeOffset? absoluteExpiration);

    /// <summary>
    /// Batch setting cache
    /// </summary>
    /// <param name="keyValues">A collection of key-value pairs</param>
    /// <param name="absoluteExpirationRelativeToNow">absolute Expiration Relative To Now，Permanently valid when null</param>
    /// <typeparam name="T"></typeparam>
    void SetList<T>(Dictionary<string, T?> keyValues, TimeSpan? absoluteExpirationRelativeToNow);

    /// <summary>
    /// Batch setting cache
    /// </summary>
    /// <param name="keyValues">A collection of key-value pairs</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <typeparam name="T"></typeparam>
    void SetList<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? options = null);

    /// <summary>
    /// Batch setting cache
    /// </summary>
    /// <param name="keyValues">A collection of key-value pairs</param>
    /// <param name="absoluteExpiration">absolute Expiration，Permanently valid when null</param>
    /// <typeparam name="T"></typeparam>
    Task SetListAsync<T>(Dictionary<string, T?> keyValues, DateTimeOffset? absoluteExpiration);

    /// <summary>
    /// Batch setting cache
    /// </summary>
    /// <param name="keyValues">A collection of key-value pairs</param>
    /// <param name="absoluteExpirationRelativeToNow">absolute Expiration Relative To Now，Permanently valid when null</param>
    /// <typeparam name="T"></typeparam>
    Task SetListAsync<T>(Dictionary<string, T?> keyValues, TimeSpan? absoluteExpirationRelativeToNow);

    /// <summary>
    /// Batch setting cache
    /// </summary>
    /// <param name="keyValues">A collection of key-value pairs</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <typeparam name="T"></typeparam>
    Task SetListAsync<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? options = null);
}
