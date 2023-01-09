// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public abstract class MultilevelCacheClientBase : CacheClientBase, IMultilevelCacheClient
{
    /// <summary>
    /// Get cache
    /// When the memory cache does not exist, get the result of the distributed cache and store the result in the memory cache (the validity period of the memory cache is the expiration time passed in)
    /// </summary>
    /// <param name="key"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public virtual T? Get<T>(string key, Action<MultilevelCacheOptions>? action = null)
        => GetCore<T>(key, null, action);

    public virtual T? Get<T>(string key, Action<T?> valueChanged, Action<MultilevelCacheOptions>? action = null)
        => GetCore(key, valueChanged, action);

    public abstract T? GetCore<T>(string key, Action<T?>? valueChanged, Action<MultilevelCacheOptions>? action = null);

    public virtual Task<T?> GetAsync<T>(string key, Action<MultilevelCacheOptions>? action = null)
        => GetCoreAsync<T>(key, null, action);

    public virtual Task<T?> GetAsync<T>(string key, Action<T?> valueChanged, Action<MultilevelCacheOptions>? action = null)
        => GetCoreAsync(key, valueChanged, action);

    public abstract Task<T?> GetCoreAsync<T>(string key, Action<T?>? valueChanged, Action<MultilevelCacheOptions>? action = null);

    public override IEnumerable<T?> GetList<T>(params string[] keys) where T : default
        => GetList<T>(GetKeys(keys));

    public abstract IEnumerable<T?> GetList<T>(IEnumerable<string> keys, Action<MultilevelCacheOptions>? action = null);

    public override Task<IEnumerable<T?>> GetListAsync<T>(params string[] keys) where T : default
        => GetListAsync<T>(GetKeys(keys));

    public abstract Task<IEnumerable<T?>> GetListAsync<T>(IEnumerable<string> keys, Action<MultilevelCacheOptions>? action = null);

    public virtual T? GetOrSet<T>(string key,
        Func<CacheEntry<T>> distributedCacheEntryFunc,
        CacheEntryOptions? memoryCacheEntryOptions = null,
        Action<CacheOptions>? action = null)
        => GetOrSet(key, new CombinedCacheEntry<T>()
        {
            DistributedCacheEntryFunc = distributedCacheEntryFunc,
            MemoryCacheEntryOptions = memoryCacheEntryOptions
        }, action);

    public abstract T? GetOrSet<T>(string key, CombinedCacheEntry<T> combinedCacheEntry, Action<CacheOptions>? action = null);

    public virtual Task<T?> GetOrSetAsync<T>(string key,
        Func<CacheEntry<T>> distributedCacheEntryFunc,
        CacheEntryOptions? memoryCacheEntryOptions = null,
        Action<CacheOptions>? action = null)
        => GetOrSetAsync(key, new CombinedCacheEntry<T>(distributedCacheEntryFunc, memoryCacheEntryOptions), action);

    public virtual Task<T?> GetOrSetAsync<T>(string key,
        Func<Task<CacheEntry<T>>> distributedCacheEntryFunc,
        CacheEntryOptions? memoryCacheEntryOptions = null,
        Action<CacheOptions>? action = null)
        => GetOrSetAsync(key, new CombinedCacheEntry<T>(distributedCacheEntryFunc, memoryCacheEntryOptions), action);

    public abstract Task<T?> GetOrSetAsync<T>(string key, CombinedCacheEntry<T> combinedCacheEntry, Action<CacheOptions>? action = null);

    public void Refresh<T>(params string[] keys) => Refresh<T>(GetKeys(keys));

    public Task RefreshAsync<T>(params string[] keys) => RefreshAsync<T>(GetKeys(keys));

    public void Remove<T>(params string[] keys)
        => Remove<T>(GetKeys(keys));

    public override void Remove<T>(string key, Action<CacheOptions>? action = null)
        => Remove<T>(new[] { key }, action);

    public Task RemoveAsync<T>(params string[] keys)
        => RemoveAsync<T>(GetKeys(keys));

    public override Task RemoveAsync<T>(string key, Action<CacheOptions>? action = null)
        => RemoveAsync<T>(new[] { key }, action);

    public virtual void Set<T>(string key,
        T value,
        CacheEntryOptions? distributedOptions,
        CacheEntryOptions? memoryOptions,
        Action<CacheOptions>? action = null)
        => Set(key, value, new CombinedCacheEntryOptions()
        {
            DistributedCacheEntryOptions = distributedOptions,
            MemoryCacheEntryOptions = memoryOptions
        }, action);

    public override void Set<T>(string key,
        T value,
        CacheEntryOptions? options = null,
        Action<CacheOptions>? action = null)
        => Set(key, value, new CombinedCacheEntryOptions()
        {
            MemoryCacheEntryOptions = options,
            DistributedCacheEntryOptions = options
        }, action);

    public abstract void Set<T>(string key, T value, CombinedCacheEntryOptions? options, Action<CacheOptions>? action = null);

    public virtual Task SetAsync<T>(string key,
        T value,
        CacheEntryOptions? distributedOptions,
        CacheEntryOptions? memoryOptions,
        Action<CacheOptions>? action = null)
        => SetAsync(key, value, new CombinedCacheEntryOptions()
        {
            DistributedCacheEntryOptions = distributedOptions,
            MemoryCacheEntryOptions = memoryOptions
        }, action);

    public override Task SetAsync<T>(string key, T value, CacheEntryOptions? options = null, Action<CacheOptions>? action = null)
        => SetAsync(key, value, new CombinedCacheEntryOptions()
        {
            MemoryCacheEntryOptions = options,
            DistributedCacheEntryOptions = options
        }, action);

    public abstract Task SetAsync<T>(string key, T value, CombinedCacheEntryOptions? options, Action<CacheOptions>? action = null);

    public virtual void SetList<T>(
        Dictionary<string, T?> keyValues,
        CacheEntryOptions? distributedOptions,
        CacheEntryOptions? memoryOptions,
        Action<CacheOptions>? action = null)
        => SetList(keyValues, new CombinedCacheEntryOptions()
        {
            DistributedCacheEntryOptions = distributedOptions,
            MemoryCacheEntryOptions = memoryOptions
        }, action);

    public override void SetList<T>(
        Dictionary<string, T?> keyValues,
        CacheEntryOptions? options = null,
        Action<CacheOptions>? action = null) where T : default
        => SetList(keyValues, new CombinedCacheEntryOptions()
        {
            MemoryCacheEntryOptions = options,
            DistributedCacheEntryOptions = options
        }, action);

    public abstract void SetList<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions? options,
        Action<CacheOptions>? action = null);

    public virtual Task SetListAsync<T>(
        Dictionary<string, T?> keyValues,
        CacheEntryOptions? distributedOptions,
        CacheEntryOptions? memoryOptions,
        Action<CacheOptions>? action = null)
        => SetListAsync(keyValues, new CombinedCacheEntryOptions()
        {
            DistributedCacheEntryOptions = distributedOptions,
            MemoryCacheEntryOptions = memoryOptions
        }, action);

    public override Task SetListAsync<T>(
        Dictionary<string, T?> keyValues,
        CacheEntryOptions? options = null,
        Action<CacheOptions>? action = null) where T : default
        => SetListAsync(keyValues, new CombinedCacheEntryOptions()
        {
            MemoryCacheEntryOptions = options,
            DistributedCacheEntryOptions = options
        }, action);

    public abstract Task SetListAsync<T>(
        Dictionary<string, T?> keyValues,
        CombinedCacheEntryOptions? options,
        Action<CacheOptions>? action = null);
}
