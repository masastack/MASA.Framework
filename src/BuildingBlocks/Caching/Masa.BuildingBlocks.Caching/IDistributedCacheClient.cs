﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public interface IDistributedCacheClient : ICacheClient
{
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

    void Remove(params string[] keys);

    Task RemoveAsync(params string[] keys);

    bool Exists(string key);

    bool Exists<T>(string key, Action<CacheOptions>? action = null);

    Task<bool> ExistsAsync(string key);

    Task<bool> ExistsAsync<T>(string key, Action<CacheOptions>? action = null);

    /// <summary>
    /// Only get the key, do not trigger the update expiration time policy
    /// </summary>
    /// <param name="keyPattern">keyPattern, such as: app_*</param>
    /// <returns></returns>
    IEnumerable<string> GetKeys(string keyPattern);

    /// <summary>
    /// The set of cached keys that match the rules according to the key fuzzy matching
    /// Obtain the set of keys that meet the rules according to the obtained type and KeyPattern
    /// </summary>
    /// <param name="keyPattern">eg: 1*</param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IEnumerable<string> GetKeys<T>(string keyPattern, Action<CacheOptions>? action = null);

    Task<IEnumerable<string>> GetKeysAsync(string keyPattern);

    Task<IEnumerable<string>> GetKeysAsync<T>(string keyPattern, Action<CacheOptions>? action = null);

    IEnumerable<KeyValuePair<string, T?>> GetByKeyPattern<T>(string keyPattern, Action<CacheOptions>? action = null);

    Task<IEnumerable<KeyValuePair<string, T?>>> GetByKeyPatternAsync<T>(string keyPattern, Action<CacheOptions>? action = null);

    void Publish(string channel, Action<PublishOptions> options);

    Task PublishAsync(string channel, Action<PublishOptions> options);

    void Subscribe<T>(string channel, Action<SubscribeOptions<T>> options);

    Task SubscribeAsync<T>(string channel, Action<SubscribeOptions<T>> options);

    Task<long> HashIncrementAsync(string key, long value = 1L, Action<CacheOptions>? action = null);

    /// <summary>
    /// Descending Hash
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="value">decrement increment, must be greater than 0</param>
    /// <param name="defaultMinVal">critical value, must be greater than or equal to 0</param>
    /// <param name="action"></param>
    /// <returns></returns>
    Task<long> HashDecrementAsync(string key, long value = 1L, long defaultMinVal = 0L, Action<CacheOptions>? action = null);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="absoluteExpirationRelativeToNow">absolute Expiration Relative To Now，Permanently valid when null</param>
    /// <returns></returns>
    bool KeyExpire(string key, TimeSpan? absoluteExpirationRelativeToNow);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="absoluteExpirationRelativeToNow">absolute Expiration Relative To Now，Permanently valid when null</param>
    /// <param name="action"></param>
    /// <returns></returns>
    bool KeyExpire<T>(string key, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="absoluteExpiration">absolute Expiration，Permanently valid when null</param>
    /// <returns></returns>
    bool KeyExpire(string key, DateTimeOffset absoluteExpiration);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="absoluteExpiration">absolute Expiration，Permanently valid when null</param>
    /// <param name="action"></param>
    /// <returns></returns>
    bool KeyExpire<T>(string key, DateTimeOffset absoluteExpiration, Action<CacheOptions>? action = null);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <returns></returns>
    bool KeyExpire(string key, CacheEntryOptions? options = null);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <param name="action"></param>
    /// <returns></returns>
    bool KeyExpire<T>(string key, CacheEntryOptions? options = null, Action<CacheOptions>? action = null);

    long KeyExpire(IEnumerable<string> keys, CacheEntryOptions? options = null);

    long KeyExpire<T>(IEnumerable<string> keys, CacheEntryOptions? options = null, Action<CacheOptions>? action = null);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="absoluteExpirationRelativeToNow">absolute Expiration Relative To Now，Permanently valid when null</param>
    /// <returns></returns>
    Task<bool> KeyExpireAsync(string key, TimeSpan? absoluteExpirationRelativeToNow);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="absoluteExpirationRelativeToNow">absolute Expiration Relative To Now，Permanently valid when null</param>
    /// <param name="action"></param>
    /// <returns></returns>
    Task<bool> KeyExpireAsync<T>(string key, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="absoluteExpiration">absolute Expiration，Permanently valid when null</param>
    /// <returns></returns>
    Task<bool> KeyExpireAsync(string key, DateTimeOffset absoluteExpiration);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="absoluteExpiration">absolute Expiration，Permanently valid when null</param>
    /// <param name="action"></param>
    /// <returns></returns>
    Task<bool> KeyExpireAsync<T>(string key, DateTimeOffset absoluteExpiration, Action<CacheOptions>? action = null);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <returns></returns>
    Task<bool> KeyExpireAsync(string key, CacheEntryOptions? options = null);

    /// <summary>
    /// Set cache lifetime
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <param name="action"></param>
    /// <returns></returns>
    Task<bool> KeyExpireAsync<T>(string key, CacheEntryOptions? options = null, Action<CacheOptions>? action = null);

    /// <summary>
    /// Batch setting cache lifetime
    /// </summary>
    /// <param name="keys">cache key</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <returns></returns>
    Task<long> KeyExpireAsync(IEnumerable<string> keys, CacheEntryOptions? options = null);

    /// <summary>
    /// Batch setting cache lifetime
    /// </summary>
    /// <param name="keys">cache key</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty</param>
    /// <param name="action"></param>
    /// <returns></returns>
    Task<long> KeyExpireAsync<T>(IEnumerable<string> keys, CacheEntryOptions? options = null, Action<CacheOptions>? action = null);
}
