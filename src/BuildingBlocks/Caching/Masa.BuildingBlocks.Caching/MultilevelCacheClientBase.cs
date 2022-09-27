// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public abstract class MultilevelCacheClientBase : CacheClientBase, IMultilevelCacheClient
{
    public abstract T? Get<T>(string key, Action<T?> valueChanged);

    public abstract Task<T?> GetAsync<T>(string key, Action<T?> valueChanged);

    public abstract T? GetOrSet<T>(string key, CombinedCacheEntry<T> combinedCacheEntry);

    public abstract Task<T?> GetOrSetAsync<T>(string key, CombinedCacheEntry<T> combinedCacheEntry);

    public void Refresh<T>(params string[] keys) => Refresh<T>(GetKeys(keys));

    public abstract void Refresh<T>(IEnumerable<string> keys);

    public Task RefreshAsync<T>(params string[] keys) => RefreshAsync<T>(GetKeys(keys));

    public abstract Task RefreshAsync<T>(IEnumerable<string> keys);

    public void Remove<T>(params string[] keys)
        => Remove<T>(GetKeys(keys));

    public abstract void Remove<T>(IEnumerable<string> keys);

    public Task RemoveAsync<T>(params string[] keys)
        => RemoveAsync<T>(GetKeys(keys));

    public abstract Task RemoveAsync<T>(IEnumerable<string> keys);

    public virtual void Set<T>(string key, T value, CacheEntryOptions? distributedOptions, CacheEntryOptions? memoryOptions)
        => Set(key, value, new CombinedCacheEntryOptions()
        {
            DistributedCacheEntryOptions = distributedOptions,
            MemoryCacheEntryOptions = memoryOptions
        });

    public override void Set<T>(string key, T value, CacheEntryOptions? options = null)
        => Set(key, value, new CombinedCacheEntryOptions()
        {
            DistributedCacheEntryOptions = options
        });

    public abstract void Set<T>(string key, T value, CombinedCacheEntryOptions? options);

    public virtual Task SetAsync<T>(string key, T value, CacheEntryOptions? distributedOptions, CacheEntryOptions? memoryOptions)
        => SetAsync(key, value, new CombinedCacheEntryOptions()
        {
            DistributedCacheEntryOptions = distributedOptions,
            MemoryCacheEntryOptions = memoryOptions
        });

    public override Task SetAsync<T>(string key, T value, CacheEntryOptions? options = null)
        => SetAsync(key, value, new CombinedCacheEntryOptions()
        {
            DistributedCacheEntryOptions = options
        });

    public abstract Task SetAsync<T>(string key, T value, CombinedCacheEntryOptions? options);

    public virtual void SetList<T>(
        Dictionary<string, T?> keyValues,
        CacheEntryOptions? distributedOptions,
        CacheEntryOptions? memoryOptions)
        => SetList(keyValues, new CombinedCacheEntryOptions()
        {
            DistributedCacheEntryOptions = distributedOptions,
            MemoryCacheEntryOptions = memoryOptions
        });

    public override void SetList<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? options = null) where T : default
        => SetList(keyValues, new CombinedCacheEntryOptions()
        {
            DistributedCacheEntryOptions = options
        });

    public abstract void SetList<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions? options);

    public virtual Task SetListAsync<T>(
        Dictionary<string, T?> keyValues,
        CacheEntryOptions? distributedOptions,
        CacheEntryOptions? memoryOptions)
        => SetListAsync(keyValues, new CombinedCacheEntryOptions()
        {
            DistributedCacheEntryOptions = distributedOptions,
            MemoryCacheEntryOptions = memoryOptions
        });

    public override Task SetListAsync<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? options = null) where T : default
        => SetListAsync(keyValues, new CombinedCacheEntryOptions()
        {
            DistributedCacheEntryOptions = options
        });

    public abstract Task SetListAsync<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions? options);
}
