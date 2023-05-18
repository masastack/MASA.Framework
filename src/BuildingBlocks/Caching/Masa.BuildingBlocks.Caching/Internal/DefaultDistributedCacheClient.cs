// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Caching.Distributed.StackExchangeRedis.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Caching;

internal sealed class DefaultDistributedCacheClient : DefaultCacheClient, IManualDistributedCacheClient
{
    private readonly IManualDistributedCacheClient _cacheClient;
    public DefaultDistributedCacheClient(IManualDistributedCacheClient cacheClient) : base(cacheClient) => _cacheClient = cacheClient;

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

#pragma warning disable S3881
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        //don't need to be released
    }
#pragma warning restore S3881
}
