// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Caching;

internal class DefaultDistributedCacheClient : IManualDistributedCacheClient
{
    private readonly IManualDistributedCacheClient _cacheClient;
    public DefaultDistributedCacheClient(IManualDistributedCacheClient cacheClient) => _cacheClient = cacheClient;

    public IEnumerable<T?> GetList<T>(params string[] keys) => _cacheClient.GetList<T>(keys);

    public Task<IEnumerable<T?>> GetListAsync<T>(params string[] keys) => _cacheClient.GetListAsync<T>(keys);

    public void Set<T>(string key, T value, DateTimeOffset? absoluteExpiration, Action<CacheOptions>? action = null)
        => _cacheClient.Set(key, value, absoluteExpiration, action);

    public void Set<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null)
        => _cacheClient.Set(key, value, absoluteExpirationRelativeToNow, action);

    public void Set<T>(string key, T value, CacheEntryOptions? options = null, Action<CacheOptions>? action = null)
        => _cacheClient.Set(key, value, options, action);

    public Task SetAsync<T>(string key, T value, DateTimeOffset? absoluteExpiration, Action<CacheOptions>? action = null)
        => _cacheClient.SetAsync(key, value, absoluteExpiration, action);

    public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null)
        => _cacheClient.SetAsync(key, value, absoluteExpirationRelativeToNow, action);

    public Task SetAsync<T>(string key, T value, CacheEntryOptions? options = null, Action<CacheOptions>? action = null)
        => _cacheClient.SetAsync(key, value, options, action);

    public void SetList<T>(Dictionary<string, T?> keyValues, DateTimeOffset? absoluteExpiration, Action<CacheOptions>? action = null)
        => _cacheClient.SetList(keyValues, absoluteExpiration, action);

    public void SetList<T>(Dictionary<string, T?> keyValues, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null)
        => _cacheClient.SetList(keyValues, absoluteExpirationRelativeToNow, action);

    public void SetList<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? options = null, Action<CacheOptions>? action = null)
        => _cacheClient.SetList(keyValues, options, action);

    public Task SetListAsync<T>(Dictionary<string, T?> keyValues, DateTimeOffset? absoluteExpiration, Action<CacheOptions>? action = null)
        => _cacheClient.SetListAsync(keyValues, absoluteExpiration, action);

    public Task SetListAsync<T>(
        Dictionary<string, T?> keyValues,
        TimeSpan? absoluteExpirationRelativeToNow,
        Action<CacheOptions>? action = null)
        => _cacheClient.SetListAsync(keyValues, absoluteExpirationRelativeToNow, action);

    public Task SetListAsync<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? options = null, Action<CacheOptions>? action = null)
        => _cacheClient.SetListAsync(keyValues, options, action);

    public void Remove<T>(string key, Action<CacheOptions>? action = null)
        => _cacheClient.Remove<T>(key, action);

    public void Remove<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
        => _cacheClient.Remove<T>(keys, action);

    public Task RemoveAsync<T>(string key, Action<CacheOptions>? action = null)
        => _cacheClient.RemoveAsync<T>(key, action);

    public Task RemoveAsync<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
        => _cacheClient.RemoveAsync<T>(keys, action);

    public void Refresh<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
        => _cacheClient.Refresh<T>(keys, action);

    public Task RefreshAsync<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
        => _cacheClient.RefreshAsync<T>(keys, action);

    public T? Get<T>(string key, Action<CacheOptions>? action = null)
        => _cacheClient.Get<T>(key, action);

    public Task<T?> GetAsync<T>(string key, Action<CacheOptions>? action = null)
        => _cacheClient.GetAsync<T>(key, action);

    public IEnumerable<T?> GetList<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
        => _cacheClient.GetList<T>(keys, action);

    public Task<IEnumerable<T?>> GetListAsync<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
        => _cacheClient.GetListAsync<T>(keys, action);

    public T? GetOrSet<T>(string key, Func<CacheEntry<T>> setter, Action<CacheOptions>? action = null)
        => _cacheClient.GetOrSet(key, setter, action);

    public Task<T?> GetOrSetAsync<T>(string key, Func<CacheEntry<T>> setter, Action<CacheOptions>? action = null)
        => _cacheClient.GetOrSetAsync(key, setter, action);

    public Task<T?> GetOrSetAsync<T>(string key, Func<Task<CacheEntry<T>>> setter, Action<CacheOptions>? action = null)
        => _cacheClient.GetOrSetAsync(key, setter, action);

    public void Refresh(params string[] keys)
        => _cacheClient.Refresh(keys);

    public Task RefreshAsync(params string[] keys)
        => _cacheClient.RefreshAsync(keys);

    public void Remove(params string[] keys)
        => _cacheClient.Remove(keys);

    public Task RemoveAsync(params string[] keys)
        => _cacheClient.RemoveAsync(keys);

    public bool Exists(string key)
        => _cacheClient.Exists(key);

    public bool Exists<T>(string key, Action<CacheOptions>? action = null)
        => _cacheClient.Exists<T>(key, action);

    public Task<bool> ExistsAsync(string key)
        => _cacheClient.ExistsAsync(key);

    public Task<bool> ExistsAsync<T>(string key, Action<CacheOptions>? action = null)
        => _cacheClient.ExistsAsync<T>(key, action);

    public IEnumerable<string> GetKeys(string keyPattern)
        => _cacheClient.GetKeys(keyPattern);

    public IEnumerable<string> GetKeys<T>(string keyPattern, Action<CacheOptions>? action = null)
        => _cacheClient.GetKeys<T>(keyPattern, action);

    public Task<IEnumerable<string>> GetKeysAsync(string keyPattern)
        => _cacheClient.GetKeysAsync(keyPattern);

    public Task<IEnumerable<string>> GetKeysAsync<T>(string keyPattern, Action<CacheOptions>? action = null)
        => _cacheClient.GetKeysAsync<T>(keyPattern, action);

    public IEnumerable<KeyValuePair<string, T?>> GetByKeyPattern<T>(string keyPattern, Action<CacheOptions>? action = null)
        => _cacheClient.GetByKeyPattern<T>(keyPattern, action);

    public Task<IEnumerable<KeyValuePair<string, T?>>> GetByKeyPatternAsync<T>(string keyPattern, Action<CacheOptions>? action = null)
        => _cacheClient.GetByKeyPatternAsync<T>(keyPattern, action);

    public void Publish(string channel, Action<PublishOptions> options)
        => _cacheClient.Publish(channel, options);

    public Task PublishAsync(string channel, Action<PublishOptions> options)
        => _cacheClient.PublishAsync(channel, options);

    public void Subscribe<T>(string channel, Action<SubscribeOptions<T>> options)
        => _cacheClient.Subscribe(channel, options);

    public Task SubscribeAsync<T>(string channel, Action<SubscribeOptions<T>> options)
        => _cacheClient.SubscribeAsync(channel, options);

    public void UnSubscribe<T>(string channel)
        => _cacheClient.UnSubscribe<T>(channel);

    public Task UnSubscribeAsync<T>(string channel)
        => _cacheClient.UnSubscribeAsync<T>(channel);

    public Task<long> HashIncrementAsync(string key, long value = 1, Action<CacheOptions>? action = null, CacheEntryOptions? options = null)
        => _cacheClient.HashIncrementAsync(key, value, action, options);

    public Task<long?> HashDecrementAsync(
        string key,
        long value = 1,
        long defaultMinVal = 0,
        Action<CacheOptions>? action = null,
        CacheEntryOptions? options = null)
        => _cacheClient.HashDecrementAsync(key, value, defaultMinVal, action, options);

    public bool KeyExpire(string key, TimeSpan? absoluteExpirationRelativeToNow)
        => _cacheClient.KeyExpire(key, absoluteExpirationRelativeToNow);

    public bool KeyExpire<T>(string key, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null)
        => _cacheClient.KeyExpire<T>(key, absoluteExpirationRelativeToNow, action);

    public bool KeyExpire(string key, DateTimeOffset absoluteExpiration)
        => _cacheClient.KeyExpire(key, absoluteExpiration);

    public bool KeyExpire<T>(string key, DateTimeOffset absoluteExpiration, Action<CacheOptions>? action = null)
        => _cacheClient.KeyExpire<T>(key, absoluteExpiration, action);

    public bool KeyExpire(string key, CacheEntryOptions? options = null)
        => _cacheClient.KeyExpire(key, options);

    public bool KeyExpire<T>(string key, CacheEntryOptions? options = null, Action<CacheOptions>? action = null)
        => _cacheClient.KeyExpire<T>(key, options, action);

    public long KeyExpire(IEnumerable<string> keys, CacheEntryOptions? options = null)
        => _cacheClient.KeyExpire(keys, options);

    public long KeyExpire<T>(IEnumerable<string> keys, CacheEntryOptions? options = null, Action<CacheOptions>? action = null)
        => _cacheClient.KeyExpire<T>(keys, options, action);

    public Task<bool> KeyExpireAsync(string key, TimeSpan? absoluteExpirationRelativeToNow)
        => _cacheClient.KeyExpireAsync(key, absoluteExpirationRelativeToNow);

    public Task<bool> KeyExpireAsync<T>(string key, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null)
        => _cacheClient.KeyExpireAsync<T>(key, absoluteExpirationRelativeToNow, action);

    public Task<bool> KeyExpireAsync(string key, DateTimeOffset absoluteExpiration)
        => _cacheClient.KeyExpireAsync(key, absoluteExpiration);

    public Task<bool> KeyExpireAsync<T>(string key, DateTimeOffset absoluteExpiration, Action<CacheOptions>? action = null)
        => _cacheClient.KeyExpireAsync<T>(key, absoluteExpiration, action);

    public Task<bool> KeyExpireAsync(string key, CacheEntryOptions? options = null)
        => _cacheClient.KeyExpireAsync(key, options);

    public Task<bool> KeyExpireAsync<T>(string key, CacheEntryOptions? options = null, Action<CacheOptions>? action = null)
        => _cacheClient.KeyExpireAsync<T>(key, options, action);

    public Task<long> KeyExpireAsync(IEnumerable<string> keys, CacheEntryOptions? options = null)
        => _cacheClient.KeyExpireAsync(keys, options);

    public Task<long> KeyExpireAsync<T>(IEnumerable<string> keys, CacheEntryOptions? options = null, Action<CacheOptions>? action = null)
        => _cacheClient.KeyExpireAsync<T>(keys, options, action);

    public void Dispose()
    {
        //don't need to be released
    }
}
