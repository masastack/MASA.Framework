// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public abstract class AbstractCacheClient : ICacheClient
{
    public abstract T? Get<T>(string key);

    public abstract Task<T?> GetAsync<T>(string key);

    public IEnumerable<T?> GetList<T>(params string[] keys)
        => GetList<T>(GetKeys(keys));

    public abstract IEnumerable<T?> GetList<T>(IEnumerable<string> keys);

    public Task<IEnumerable<T?>> GetListAsync<T>(params string[] keys)
        => GetListAsync<T>(GetKeys(keys));

    public abstract Task<IEnumerable<T?>> GetListAsync<T>(IEnumerable<string> keys);

    public virtual void Set<T>(string key, T value, DateTimeOffset absoluteExpiration)
        => Set(key, value, new CacheEntryOptions(absoluteExpiration));

    public virtual Task SetAsync<T>(string key, T value, DateTimeOffset absoluteExpiration)
        => SetAsync(key, value, new CacheEntryOptions(absoluteExpiration));

    public virtual void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
        => Set(key, value, new CacheEntryOptions(absoluteExpirationRelativeToNow));

    public virtual Task SetAsync<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
        => SetAsync(key, value, new CacheEntryOptions(absoluteExpirationRelativeToNow));

    public abstract void Set<T>(string key, T value, CacheEntryOptions? options = null);

    public abstract Task SetAsync<T>(string key, T value, CacheEntryOptions? options = null);

    public virtual void SetList<T>(Dictionary<string, T?> keyValues, DateTimeOffset absoluteExpiration)
        => SetList(keyValues, new CacheEntryOptions(absoluteExpiration));

    public virtual Task SetListAsync<T>(Dictionary<string, T?> keyValues, DateTimeOffset absoluteExpiration)
        => SetListAsync(keyValues, new CacheEntryOptions(absoluteExpiration));

    public virtual void SetList<T>(Dictionary<string, T?> keyValues, TimeSpan absoluteExpirationRelativeToNow)
        => SetList(keyValues, new CacheEntryOptions(absoluteExpirationRelativeToNow));

    public virtual Task SetListAsync<T>(Dictionary<string, T?> keyValues, TimeSpan absoluteExpirationRelativeToNow)
        => SetListAsync(keyValues, new CacheEntryOptions(absoluteExpirationRelativeToNow));

    public abstract void SetList<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? options = null);

    public abstract Task SetListAsync<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? options = null);

    protected static IEnumerable<string> GetKeys(params string[] keys) => keys;
}
