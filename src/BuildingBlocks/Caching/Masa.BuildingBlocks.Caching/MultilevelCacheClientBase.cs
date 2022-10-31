﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public abstract class MultilevelCacheClientBase : CacheClientBase, IMultilevelCacheClient
{
    public abstract T? Get<T>(string key, Action<T?> valueChanged, Action<CacheOptions>? action = null);

    public abstract Task<T?> GetAsync<T>(string key, Action<T?> valueChanged, Action<CacheOptions>? action = null);

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
        => GetOrSetAsync(key, new CombinedCacheEntry<T>()
        {
            DistributedCacheEntryFunc = distributedCacheEntryFunc,
            MemoryCacheEntryOptions = memoryCacheEntryOptions
        }, action);

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
            DistributedCacheEntryOptions = options
        }, action);

    public abstract Task SetListAsync<T>(
        Dictionary<string, T?> keyValues,
        CombinedCacheEntryOptions? options,
        Action<CacheOptions>? action = null);
}
