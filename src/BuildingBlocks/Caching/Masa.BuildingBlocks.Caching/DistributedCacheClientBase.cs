// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public abstract class DistributedCacheClientBase : CacheClientBase, IDistributedCacheClient
{
    public abstract T? GetOrSet<T>(string key, Func<CacheEntry<T>> setter, Action<CacheOptions>? action = null);

    public abstract Task<T?> GetOrSetAsync<T>(string key, Func<CacheEntry<T>> setter, Action<CacheOptions>? action = null);

    public abstract void Refresh(params string[] keys);

    public abstract Task RefreshAsync(params string[] keys);

    public abstract void Remove(params string[] keys);

    public override void Remove<T>(string key, Action<CacheOptions>? action = null)
        => Remove<T>(new[] { key }, action);

    public abstract Task RemoveAsync(params string[] keys);

    public override Task RemoveAsync<T>(string key, Action<CacheOptions>? action = null)
        => RemoveAsync<T>(new[] { key }, action);

    public abstract bool Exists(string key);

    public abstract bool Exists<T>(string key, Action<CacheOptions>? action = null);

    public abstract Task<bool> ExistsAsync(string key);

    public abstract Task<bool> ExistsAsync<T>(string key, Action<CacheOptions>? action = null);

    public abstract IEnumerable<string> GetKeys(string keyPattern);

    public abstract IEnumerable<string> GetKeys<T>(string keyPattern, Action<CacheOptions>? action = null);

    public abstract Task<IEnumerable<string>> GetKeysAsync(string keyPattern);

    public abstract Task<IEnumerable<string>> GetKeysAsync<T>(string keyPattern, Action<CacheOptions>? action = null);

    public abstract IEnumerable<KeyValuePair<string, T?>> GetByKeyPattern<T>(string keyPattern, Action<CacheOptions>? action = null);

    public abstract Task<IEnumerable<KeyValuePair<string, T?>>> GetByKeyPatternAsync<T>(string keyPattern,
        Action<CacheOptions>? action = null);

    public abstract void Publish(string channel, Action<PublishOptions> options);

    public abstract Task PublishAsync(string channel, Action<PublishOptions> options);

    public abstract void Subscribe<T>(string channel, Action<SubscribeOptions<T>> options);

    public abstract Task SubscribeAsync<T>(string channel, Action<SubscribeOptions<T>> options);

    public abstract Task<long> HashIncrementAsync(
        string key,
        long value = 1,
        Action<CacheOptions>? action = null,
        CacheEntryOptions? options = null);

    public abstract Task<long?> HashDecrementAsync(string key,
        long value = 1L,
        long defaultMinVal = 0L,
        Action<CacheOptions>? action = null,
        CacheEntryOptions? options = null);

    public virtual bool KeyExpire(string key, TimeSpan? absoluteExpirationRelativeToNow)
        => KeyExpire(key, new CacheEntryOptions(absoluteExpirationRelativeToNow));

    public virtual bool KeyExpire<T>(string key, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null)
        => KeyExpire<T>(key, new CacheEntryOptions(absoluteExpirationRelativeToNow), action);

    public virtual bool KeyExpire(string key, DateTimeOffset absoluteExpiration)
        => KeyExpire(key, new CacheEntryOptions(absoluteExpiration));

    public virtual bool KeyExpire<T>(string key, DateTimeOffset absoluteExpiration, Action<CacheOptions>? action = null)
        => KeyExpire<T>(key, new CacheEntryOptions(absoluteExpiration), action);

    public abstract bool KeyExpire(string key, CacheEntryOptions? options = null);

    public abstract bool KeyExpire<T>(string key, CacheEntryOptions? options = null, Action<CacheOptions>? action = null);

    public abstract long KeyExpire(IEnumerable<string> keys, CacheEntryOptions? options = null);

    public abstract long KeyExpire<T>(IEnumerable<string> keys, CacheEntryOptions? options = null, Action<CacheOptions>? action = null);

    public virtual Task<bool> KeyExpireAsync(string key, DateTimeOffset absoluteExpiration)
        => KeyExpireAsync(key, new CacheEntryOptions(absoluteExpiration));

    public virtual Task<bool> KeyExpireAsync<T>(string key, DateTimeOffset absoluteExpiration, Action<CacheOptions>? action = null)
        => KeyExpireAsync<T>(key, new CacheEntryOptions(absoluteExpiration), action);

    public virtual Task<bool> KeyExpireAsync(string key, TimeSpan? absoluteExpirationRelativeToNow)
        => KeyExpireAsync(key, new CacheEntryOptions(absoluteExpirationRelativeToNow));

    public virtual Task<bool> KeyExpireAsync<T>(string key, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null)
        => KeyExpireAsync<T>(key, new CacheEntryOptions(absoluteExpirationRelativeToNow), action);

    public abstract Task<bool> KeyExpireAsync(string key, CacheEntryOptions? options = null);

    public abstract Task<bool> KeyExpireAsync<T>(string key, CacheEntryOptions? options = null, Action<CacheOptions>? action = null);

    public abstract Task<long> KeyExpireAsync(
        IEnumerable<string> keys,
        CacheEntryOptions? options = null);

    public abstract Task<long> KeyExpireAsync<T>(
        IEnumerable<string> keys,
        CacheEntryOptions? options = null,
        Action<CacheOptions>? action = null);
}
