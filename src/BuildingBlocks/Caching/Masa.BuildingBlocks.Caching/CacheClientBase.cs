// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public abstract class CacheClientBase : ICacheClient
{
    public abstract IEnumerable<T?> GetList<T>(params string[] keys);

    public abstract Task<IEnumerable<T?>> GetListAsync<T>(params string[] keys);

    public virtual void Set<T>(string key, T value, DateTimeOffset? absoluteExpiration, Action<CacheOptions>? action = null)
        => Set(key, value, new CacheEntryOptions(absoluteExpiration), action);

    public virtual void Set<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null)
        => Set(key, value, new CacheEntryOptions(absoluteExpirationRelativeToNow), action);

    public abstract void Set<T>(string key, T value, CacheEntryOptions? options = null, Action<CacheOptions>? action = null);

    public virtual Task SetAsync<T>(string key, T value, DateTimeOffset? absoluteExpiration, Action<CacheOptions>? action = null)
        => SetAsync(key, value, new CacheEntryOptions(absoluteExpiration), action);

    public virtual Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null)
        => SetAsync(key, value, new CacheEntryOptions(absoluteExpirationRelativeToNow), action);

    public abstract Task SetAsync<T>(string key, T value, CacheEntryOptions? options = null, Action<CacheOptions>? action = null);

    public virtual void SetList<T>(Dictionary<string, T?> keyValues,
        DateTimeOffset? absoluteExpiration,
        Action<CacheOptions>? action = null)
        => SetList(keyValues, new CacheEntryOptions(absoluteExpiration), action);

    public virtual void SetList<T>(Dictionary<string, T?> keyValues,
        TimeSpan? absoluteExpirationRelativeToNow,
        Action<CacheOptions>? action = null)
        => SetList(keyValues, new CacheEntryOptions(absoluteExpirationRelativeToNow), action);

    public abstract void SetList<T>(Dictionary<string, T?> keyValues,
        CacheEntryOptions? options = null,
        Action<CacheOptions>? action = null);

    public virtual Task SetListAsync<T>(Dictionary<string, T?> keyValues,
        DateTimeOffset? absoluteExpiration,
        Action<CacheOptions>? action = null)
        => SetListAsync(keyValues, new CacheEntryOptions(absoluteExpiration), action);

    public virtual Task SetListAsync<T>(Dictionary<string, T?> keyValues,
        TimeSpan? absoluteExpirationRelativeToNow,
        Action<CacheOptions>? action = null)
        => SetListAsync(keyValues, new CacheEntryOptions(absoluteExpirationRelativeToNow), action);

    public abstract Task SetListAsync<T>(Dictionary<string, T?> keyValues,
        CacheEntryOptions? options = null,
        Action<CacheOptions>? action = null);

    public abstract void Remove<T>(string key, Action<CacheOptions>? action = null);

    public abstract void Remove<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null);

    public abstract Task RemoveAsync<T>(string key, Action<CacheOptions>? action = null);

    public abstract Task RemoveAsync<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null);

    public abstract void Refresh<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null);

    public abstract Task RefreshAsync<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null);

    protected static IEnumerable<string> GetKeys(params string[] keys) => keys;
}
