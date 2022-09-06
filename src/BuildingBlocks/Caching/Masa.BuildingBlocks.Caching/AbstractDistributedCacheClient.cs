﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public abstract class AbstractDistributedCacheClient : AbstractCacheClient, IDistributedCacheClient
{
    public abstract T? GetOrSet<T>(string key, Func<CacheEntry<T>> setter);

    public abstract Task<T?> GetOrSetAsync<T>(string key, Func<CacheEntry<T>> setter);

    public void Refresh(params string[] keys) => Refresh(GetKeys(keys));

    public abstract void Refresh(IEnumerable<string> keys);

    public Task RefreshAsync(params string[] keys) => RefreshAsync(GetKeys(keys));

    public abstract Task RefreshAsync(IEnumerable<string> keys);

    public virtual void Remove(params string[] keys) => Remove(GetKeys(keys));

    public abstract void Remove(IEnumerable<string> keys);

    public Task RemoveAsync(params string[] keys) => RemoveAsync(GetKeys(keys));

    public abstract Task RemoveAsync(IEnumerable<string> keys);

    public abstract bool Exists(string key);

    public abstract Task<bool> ExistsAsync(string key);

    public abstract List<string> GetKeys(string keyPattern);

    public abstract Task<List<string>> GetKeysAsync(string keyPattern);

    public abstract List<KeyValuePair<string, T?>> GetListByKeyPattern<T>(string keyPattern);

    public abstract Task<List<KeyValuePair<string, T?>>> GetListByKeyPatternAsync<T>(string keyPattern);

    public abstract void Publish(string channel, Action<PublishOptions> setup);

    public abstract Task PublishAsync(string channel, Action<PublishOptions> setup);

    public abstract void Subscribe<T>(string channel, Action<SubscribeOptions<T>> handler);

    public abstract Task SubscribeAsync<T>(string channel, Action<SubscribeOptions<T>> handler);

    public abstract Task<long> HashIncrementAsync(string key, long value = 1);

    public abstract Task<long> HashDecrementAsync(string key, long value = 1, long defaultMinVal = 0);

    public virtual bool KeyExpire(string key, DateTimeOffset absoluteExpiration)
        => KeyExpire(key, new CacheEntryOptions(absoluteExpiration));

    public virtual bool KeyExpire(string key, TimeSpan absoluteExpirationRelativeToNow)
        => KeyExpire(key, new CacheEntryOptions(absoluteExpirationRelativeToNow));

    public abstract bool KeyExpire(string key, CacheEntryOptions? options = null);

    public abstract long KeyExpire(IEnumerable<string> keys, CacheEntryOptions? options = null);

    public virtual Task<bool> KeyExpireAsync(string key, TimeSpan absoluteExpirationRelativeToNow)
        => KeyExpireAsync(key, new CacheEntryOptions(absoluteExpirationRelativeToNow));

    public virtual Task<bool> KeyExpireAsync(string key, DateTimeOffset absoluteExpiration)
        => KeyExpireAsync(key, new CacheEntryOptions(absoluteExpiration));

    public abstract Task<bool> KeyExpireAsync(string key, CacheEntryOptions? options = null);

    public abstract Task<long> KeyExpireAsync(IEnumerable<string> keys, CacheEntryOptions? options = null);
}
