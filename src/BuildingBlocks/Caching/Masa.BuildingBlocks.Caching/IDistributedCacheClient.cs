// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public interface IDistributedCacheClient : ICacheClient
{
    T? GetOrSet<T>(string key, Func<CacheEntry<T>> setter);

    Task<T?> GetOrSetAsync<T>(string key, Func<CacheEntry<T>> setter);

    /// <summary>
    /// Flush cache time to live
    /// </summary>
    /// <param name="keys"></param>
    void Refresh(params string[] keys);

    /// <summary>
    /// Flush cache time to live
    /// </summary>
    /// <param name="keys"></param>
    void Refresh(IEnumerable<string> keys);

    /// <summary>
    /// Flush cache time to live
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    Task RefreshAsync(params string[] keys);

    /// <summary>
    /// Flush cache time to live
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    Task RefreshAsync(IEnumerable<string> keys);

    void Remove(params string[] keys);

    void Remove(IEnumerable<string> keys);

    Task RemoveAsync(params string[] keys);

    Task RemoveAsync(IEnumerable<string> keys);

    bool Exists(string key);

    Task<bool> ExistsAsync(string key);

    IEnumerable<string> GetKeys(string keyPattern);

    Task<IEnumerable<string>> GetKeysAsync(string keyPattern);

    IEnumerable<KeyValuePair<string, T?>> GetByKeyPattern<T>(string keyPattern);

    Task<IEnumerable<KeyValuePair<string, T?>>> GetByKeyPatternAsync<T>(string keyPattern);

    void Publish(string channel, Action<PublishOptions> options);

    Task PublishAsync(string channel, Action<PublishOptions> options);

    void Subscribe<T>(string channel, Action<SubscribeOptions<T>> options);

    Task SubscribeAsync<T>(string channel, Action<SubscribeOptions<T>> options);

    Task<long> HashIncrementAsync(string key, long value = 1L);

    /// <summary>
    /// Descending Hash
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="value">decrement increment, must be greater than 0</param>
    /// <param name="defaultMinVal">critical value, must be greater than or equal to 0</param>
    /// <returns></returns>
    Task<long> HashDecrementAsync(string key, long value = 1L, long defaultMinVal = 0L);

    bool KeyExpire(string key, TimeSpan absoluteExpirationRelativeToNow);

    bool KeyExpire(string key, DateTimeOffset absoluteExpiration);

    bool KeyExpire(string key, CacheEntryOptions? options = null);

    long KeyExpire(IEnumerable<string> keys, CacheEntryOptions? options = null);

    Task<bool> KeyExpireAsync(string key, TimeSpan absoluteExpirationRelativeToNow);

    Task<bool> KeyExpireAsync(string key, DateTimeOffset absoluteExpiration);

    Task<bool> KeyExpireAsync(string key, CacheEntryOptions? options = null);

    Task<long> KeyExpireAsync(IEnumerable<string> keys, CacheEntryOptions? options = null);
}
